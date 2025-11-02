const API_BASE = '/api';

const state = {
    user: null,
    products: [],
    categories: [],
    users: [],
    currentPage: 'products',
    pagination: {
        currentPage: 1,
        pageSize: 3,
        totalCount: 0
    },
    filters: {
        name: '',
        categoryId: '',
        minPrice: '',
        maxPrice: '',
        orderBy: 'Name',
        direction: 'asc'
    }
};

const mainContainer = document.getElementById('main-container');
const userInfo = document.getElementById('user-info');
const username = document.getElementById('username');
const loginBtn = document.getElementById('login-btn');
const registerBtn = document.getElementById('register-btn');
const logoutBtn = document.getElementById('logout-btn');
const productModal = document.getElementById('product-modal');
const categoryModal = document.getElementById('category-modal');
const userModal = document.getElementById('user-modal');
const changePasswordModal = document.getElementById('change-password-modal');
const adminOnlyElements = document.querySelectorAll('.admin-only');
const proAdminOnlyElements = document.querySelectorAll('.pro-admin-only');

const api = (path, options = {}) => {
    return fetch(`${API_BASE}${path}`, {
        credentials: 'include',
        ...options
    });
};

const handleApiError = async (res) => {
    let errorData;
    try {
        errorData = await res.json();
    } catch {
        errorData = { message: await res.text() };
    }
    throw { status: res.status, data: errorData };
};

async function init() {
    await checkAuth();
    attachEventListeners();
    updateNavigationVisibility();
    navigateTo('products');
    document.querySelector('.nav-link[data-page="products"]')?.classList.add('active');
}

function updateNavigationVisibility() {
    const isAdmin = state.user?.role === 'Admin';
    const isProOrAdmin = isAdmin || state.user?.role === 'ProUser';

    document.querySelectorAll('.nav-link.admin-only').forEach(el => {
        el.classList.toggle('hidden', !isAdmin);
        el.style.display = el.classList.contains('hidden') ? 'none' : '';
    });

    document.querySelectorAll('.nav-link.pro-admin-only').forEach(el => {
        el.classList.toggle('hidden', !isProOrAdmin);
        el.style.display = el.classList.contains('hidden') ? 'none' : '';
    });
}

async function checkAuth() {
    try {
        const res = await api('/auth/me');
        if (res.ok) {
            state.user = await res.json();
            userInfo.classList.remove('hidden');
            loginBtn.classList.add('hidden');
            registerBtn.classList.add('hidden');
            username.textContent = state.user.name;
        } else {
            state.user = null;
            userInfo.classList.add('hidden');
            loginBtn.classList.remove('hidden');
            registerBtn.classList.remove('hidden');
        }
        updateUIByRole();
        updateNavigationVisibility();
    } catch (error) {
        console.error('Auth check failed:', error);
        state.user = null;
        updateNavigationVisibility();
    }
}

function updateUIByRole() {
    if (!state.user) return;
    const { role } = state.user;
    adminOnlyElements.forEach(el => el.classList.toggle('hidden', role !== 'Admin'));
    proAdminOnlyElements.forEach(el => el.classList.toggle('hidden', !(role === 'Admin' || role === 'ProUser')));
}

function attachEventListeners() {
    document.querySelectorAll('[data-page]').forEach(el => {
        el.addEventListener('click', e => {
            e.preventDefault();
            navigateTo(el.dataset.page);
        });
    });
    logoutBtn.addEventListener('click', logout);
    document.querySelectorAll('.close-modal').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.modal').forEach(m => m.classList.remove('show'));
        });
    });
    document.getElementById('product-form').addEventListener('submit', handleProductSubmit);
    document.getElementById('category-form').addEventListener('submit', handleCategorySubmit);
    document.getElementById('user-form').addEventListener('submit', handleUserSubmit);
    document.getElementById('change-password-form').addEventListener('submit', handlePasswordChange);
}

function navigateTo(page) {
    state.currentPage = page;
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.toggle('active', link.dataset.page === page);
    });

    switch (page) {
        case 'products': loadProductsPage(); break;
        case 'categories': loadCategoriesPage(); break;
        case 'users': loadUsersPage(); break;
        case 'login': loadLoginPage(); break;
        case 'register': loadRegisterPage(); break;
        case 'logs': loadLogsPage(); break;
        default: loadProductsPage();
    }
}

function buildProductsUrl() {
    const params = new URLSearchParams();
    params.append('page', state.pagination.currentPage);
    params.append('pageSize', state.pagination.pageSize);
    if (state.filters.name) params.append('name', state.filters.name);
    if (state.filters.categoryId) params.append('categoryId', state.filters.categoryId);
    if (state.filters.minPrice) params.append('minPrice', state.filters.minPrice);
    if (state.filters.maxPrice) params.append('maxPrice', state.filters.maxPrice);
    if (state.filters.orderBy) params.append('orderby', state.filters.orderBy);
    if (state.filters.direction) params.append('direction', state.filters.direction);
    return `/products?${params.toString()}`;
}

async function loadProductsPage() {
    mainContainer.innerHTML = `
        <div class="card">
            <div class="card-header">
                <h1 class="card-title">Продукты</h1>
                <div class="auth-only hidden">
                    <button class="btn btn-primary" id="add-product-btn">Добавить продукт</button>
                </div>
            </div>
            <div class="card-body">
                <div class="filters-section" style="margin-bottom: 20px; padding: 15px; background: #f8f9fa; border-radius: 6px;">
                    <div style="display: grid; grid-template-columns: 1.5fr 1fr auto auto auto auto auto; gap: 10px; align-items: center;">
                        <div><input type="text" id="search-input" placeholder="Поиск..." style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;"></div>
                        <div><select id="category-filter" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;"><option value="">Все категории</option></select></div>
                        <div style="display: flex; align-items: center; gap: 5px; white-space: nowrap;">
                            <span style="font-size: 0.9rem;">Цена от</span>
                            <input type="number" id="min-price" placeholder="0" min="0" max="1000000" style="width: 100px; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                        </div>
                        <div style="display: flex; align-items: center; gap: 5px; white-space: nowrap;">
                            <span style="font-size: 0.9rem;">Цена до</span>
                            <input type="number" id="max-price" placeholder="1000000" min="0" max="1000000" style="width: 100px; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                        </div>
                        <div><select id="price-sort" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="">Без сортировки</option>
                            <option value="Price-asc">По цене (↑)</option>
                            <option value="Price-desc">По цене (↓)</option>
                        </select></div>
                        <div><button class="btn btn-primary" id="apply-filters" style="padding: 8px 16px;">Применить</button></div>
                        <div><button class="btn btn-light" id="reset-filters" style="padding: 8px 16px;">Сброс</button></div>
                    </div>
                </div>

                <div id="products-container" class="grid">
                    <div class="loading">Загрузка продуктов...</div>
                </div>

                <div class="pagination" style="margin-top: 20px; display: flex; justify-content: center; align-items: center; gap: 10px; flex-wrap: wrap;">
                    <button class="btn btn-light" id="prev-page">Назад</button>
                    <span>Страница <strong id="current-page-num">1</strong></span>
                    <button class="btn btn-light" id="next-page">Вперёд</button>
                    <span style="margin-left: 10px; color: #666;" id="total-info">Всего: 0</span>
                </div>
            </div>
        </div>
    `;

    if (state.user) document.querySelector('.auth-only').classList.remove('hidden');

    document.getElementById('add-product-btn')?.addEventListener('click', () => openProductModal());
    document.getElementById('apply-filters').addEventListener('click', applyProductFilters);
    document.getElementById('reset-filters').addEventListener('click', resetProductFilters);
    document.getElementById('prev-page').addEventListener('click', () => changePage(-1));
    document.getElementById('next-page').addEventListener('click', () => changePage(1));
    document.getElementById('search-input').addEventListener('keypress', e => e.key === 'Enter' && applyProductFilters());

    try {
        await Promise.all([fetchCategories(), fetchProducts()]);
        populateCategoryFilter();
        renderProducts();
        updatePaginationInfo();
    } catch {
        document.getElementById('products-container').innerHTML = `<div class="alert alert-danger">Ошибка загрузки</div>`;
    }
}

function applyProductFilters() {
    state.filters.name = document.getElementById('search-input').value.trim();
    state.filters.categoryId = document.getElementById('category-filter').value;
    state.filters.minPrice = document.getElementById('min-price').value;
    state.filters.maxPrice = document.getElementById('max-price').value;

    const sortValue = document.getElementById('price-sort').value;
    if (sortValue) {
        const [orderBy, direction] = sortValue.split('-');
        state.filters.orderBy = orderBy;
        state.filters.direction = direction;
    } else {
        state.filters.orderBy = '';
        state.filters.direction = '';
    }

    state.pagination.currentPage = 1;
    fetchProducts().then(() => {
        renderProducts();
        updatePaginationInfo();
        document.getElementById('current-page-num').textContent = '1';
    });
}

function resetProductFilters() {
    document.getElementById('search-input').value = '';
    document.getElementById('category-filter').value = '';
    document.getElementById('min-price').value = '';
    document.getElementById('max-price').value = '';
    document.getElementById('price-sort').value = '';

    state.filters = { name: '', categoryId: '', minPrice: '', maxPrice: '', orderBy: '', direction: '' };
    state.pagination.currentPage = 1;

    fetchProducts().then(() => {
        renderProducts();
        updatePaginationInfo();
        document.getElementById('current-page-num').textContent = '1';
    });
}

function changePage(delta) {
    const newPage = state.pagination.currentPage + delta;
    if (newPage < 1 || (newPage - 1) * state.pagination.pageSize >= state.pagination.totalCount) return;
    state.pagination.currentPage = newPage;
    fetchProducts().then(() => {
        renderProducts();
        updatePaginationInfo();
        document.getElementById('current-page-num').textContent = newPage;
    });
}

function updatePaginationInfo() {
    const start = (state.pagination.currentPage - 1) * state.pagination.pageSize + 1;
    const end = Math.min(state.pagination.currentPage * state.pagination.pageSize, state.pagination.totalCount);
    document.getElementById('total-info').textContent = `Показано ${start}–${end} из ${state.pagination.totalCount}`;
    document.getElementById('prev-page').style.display = state.pagination.currentPage === 1 ? 'none' : 'inline-block';
    document.getElementById('next-page').style.display = state.pagination.currentPage * state.pagination.pageSize >= state.pagination.totalCount ? 'none' : 'inline-block';
}

function populateCategoryFilter() {
    const select = document.getElementById('category-filter');
    select.innerHTML = '<option value="">Все категории</option>';
    state.categories.forEach(cat => {
        const opt = document.createElement('option');
        opt.value = cat.id;
        opt.textContent = cat.name;
        select.appendChild(opt);
    });
}

async function fetchProducts() {
    const url = buildProductsUrl();
    const res = await api(url);
    if (!res.ok) throw new Error('Failed');
    const result = await res.json();
    state.products = result.data || [];
    state.pagination.totalCount = result.totalCount || 0;
    return result;
}

function renderProducts() {
    const container = document.getElementById('products-container');
    if (!state.products.length) {
        container.innerHTML = `<div class="alert alert-info" style="grid-column: 1/-1;">Нет продуктов</div>`;
        return;
    }

    container.innerHTML = state.products.map(p => {
        const catName = p.categoryName || 'Неизвестная';
        const editBtn = state.user ? `<button class="btn btn-light edit-product" data-id="${p.id}">Редактировать</button>` : '';
        const delBtn = (state.user && (state.user.role === 'Admin' || state.user.role === 'ProUser')) ? `<button class="btn btn-danger delete-product" data-id="${p.id}">Удалить</button>` : '';
        const special = (p.specialNote && (state.user?.role === 'Admin' || state.user?.role === 'ProUser')) ? `<div style="margin-top:8px;padding:6px;background:#fff3cd;border-radius:4px;font-size:0.9rem;"><strong>Специальное:</strong> ${p.specialNote}</div>` : '';

        return `
            <div class="card product-card">
                <div class="card-body">
                    <h3 class="product-title">${p.name}</h3>
                    <div style="color:#666;font-size:0.9rem;margin-bottom:8px;">Категория: ${catName}</div>
                    <div class="product-description">${p.description}</div>
                    <div class="product-price" style="display: flex; align-items: center; gap: 8px;">
                        ${Number(p.price).toFixed(2)} руб.
                        <span class="price-info" data-product-price="${p.price}" style="position: relative; cursor: pointer; color: #3498db;">
                            *
                            <span class="price-tooltip" style="position: absolute; bottom: 100%; left: 50%; transform: translateX(-50%); background: #333; color: white; padding: 5px 10px; border-radius: 4px; font-size: 12px; white-space: nowrap; opacity: 0; visibility: hidden; transition: opacity 0.3s; z-index: 1000;">
                                Загрузка...
                            </span>
                        </span>
                    </div>
                    ${p.note ? `<div style="margin-top:8px;font-style:italic;color:#666;">Примечание: ${p.note}</div>` : ''}
                    ${special}
                    <div class="product-actions" style="margin-top:12px;">
                        ${editBtn} ${delBtn}
                    </div>
                </div>
            </div>
        `;
    }).join('');

    document.querySelectorAll('.price-info').forEach(icon => {
        const tooltip = icon.querySelector('.price-tooltip');
        const productPrice = parseFloat(icon.dataset.productPrice);

        icon.addEventListener('mouseenter', async () => {
            tooltip.style.opacity = '1';
            tooltip.style.visibility = 'visible';
            try {
                const currencyData = await fetchCurrencyRate(431);
                const priceInUsd = productPrice / currencyData.rate;
                tooltip.textContent = `$${priceInUsd.toFixed(2)} (курс: ${currencyData.rate.toFixed(2)})`;
            } catch {
                tooltip.textContent = 'Ошибка загрузки курса';
            }
        });

        icon.addEventListener('mouseleave', () => {
            tooltip.style.opacity = '0';
            tooltip.style.visibility = 'hidden';
            tooltip.textContent = 'Загрузка...';
        });
    });

    document.querySelectorAll('.edit-product').forEach(b => b.addEventListener('click', () => {
        const prod = state.products.find(x => x.id === b.dataset.id);
        openProductModal(prod);
    }));
    document.querySelectorAll('.delete-product').forEach(b => b.addEventListener('click', () => {
        if (confirm('Удалить?')) {
            deleteProduct(b.dataset.id).then(() => fetchProducts().then(() => { renderProducts(); updatePaginationInfo(); }));
        }
    }));
}

async function fetchCurrencyRate(currencyId) {
    const res = await api(`/currencies/${currencyId}`);
    if (!res.ok) throw new Error('Failed');
    return await res.json();
}

function openProductModal(product = null) {
    const title = document.getElementById('product-modal-title');
    const idInp = document.getElementById('product-id');
    const nameInp = document.getElementById('product-name');
    const catSel = document.getElementById('product-category');
    const descInp = document.getElementById('product-description');
    const priceInp = document.getElementById('product-price');
    const noteInp = document.getElementById('product-note');
    const specInp = document.getElementById('product-special-note');

    catSel.innerHTML = state.categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');

    if (product) {
        title.textContent = 'Редактировать продукт';
        idInp.value = product.id;
        nameInp.value = product.name;
        catSel.value = product.categoryId;
        descInp.value = product.description;
        priceInp.value = product.price;
        noteInp.value = product.note || '';
        specInp.value = product.specialNote || '';
    } else {
        title.textContent = 'Добавить продукт';
        idInp.value = '';
        nameInp.value = '';
        catSel.value = state.categories[0]?.id || '';
        descInp.value = '';
        priceInp.value = '';
        noteInp.value = '';
        specInp.value = '';
    }

    productModal.classList.add('show');
}

async function handleProductSubmit(e) {
    e.preventDefault();
    const id = document.getElementById('product-id').value;
    const data = {
        name: document.getElementById('product-name').value,
        categoryId: document.getElementById('product-category').value,
        description: document.getElementById('product-description').value,
        price: parseFloat(document.getElementById('product-price').value),
        note: document.getElementById('product-note').value || null,
        specialNote: document.getElementById('product-special-note').value || null
    };

    try {
        if (id) await updateProduct(id, data);
        else await createProduct(data);

        productModal.classList.remove('show');
        await fetchProducts();
        renderProducts();
        updatePaginationInfo();
    } catch (err) {
        const message = formatErrorMessage(err);
        alert(message);
    }
}

const formatErrorMessage = (err) => {
    let message = 'Неизвестная ошибка';
    if (err.status) {
        const { status, data } = err;
        if (status === 423) message = 'Ваш аккаунт заблокирован.';
        else if (status === 401) message = 'Сессия истекла. Войдите заново.';
        else if (status === 400 && data?.errors) {
            message = `Ошибки валидации:\n${Object.values(data.errors).flat().join('\n')}`;
        } else if (data?.error) message = data.error;
        else if (data?.title) message = data.title;
        else if (data?.detail) message = data.detail;
        else message = `Ошибка ${status}`;
    } else if (err.message) message = err.message;
    return message;
};

async function createProduct(data) {
    const res = await api('/products', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    if (!res.ok) await handleApiError(res);
    return await res.json();
}

async function updateProduct(id, data) {
    const res = await api(`/products/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    if (!res.ok) await handleApiError(res);
    return res.status === 204 ? null : await res.json();
}

async function deleteProduct(id) {
    const res = await api(`/products/${id}`, { method: 'DELETE' });
    if (!res.ok) await handleApiError(res);
}

async function fetchCategories() {
    const res = await api('/categories');
    if (!res.ok) {
        const text = await res.text();
        throw new Error();
    }
    const data = await res.json();
    state.categories = data.sort((a, b) => a.name.localeCompare(b.name));
}

async function loadCategoriesPage() {
    if (!state.user || !(state.user.role === 'Admin' || state.user.role === 'ProUser')) {
        return navigateTo('products');
    }

    mainContainer.innerHTML = `
        <div class="card">
            <div class="card-header">
                <h1 class="card-title">Категории</h1>
                <div class="pro-admin-only">
                    <button class="btn btn-primary" id="add-category-btn">Добавить категорию</button>
                </div>
            </div>
            <div class="card-body">
                <div id="categories-container"><div class="loading">Загрузка...</div></div>
            </div>
        </div>
    `;

    document.querySelector('.pro-admin-only').classList.remove('hidden');
    document.getElementById('add-category-btn')?.addEventListener('click', () => openCategoryModal());
    await fetchCategories();
    renderCategories();
}

function renderCategories() {
    const container = document.getElementById('categories-container');
    if (!state.categories.length) {
        container.innerHTML = `<div class="alert alert-info">Нет категорий</div>`;
        return;
    }

    container.innerHTML = `
        <table class="table">
            <thead><tr><th>Название</th><th>Действия</th></tr></thead>
            <tbody>
                ${state.categories.map(c => `
                    <tr>
                        <td>${c.name}</td>
                        <td>
                            ${(state.user && (state.user.role === 'Admin' || state.user.role === 'ProUser')) ? `
                                <button class="btn btn-light edit-category" data-id="${c.id}">Редактировать</button>
                                <button class="btn btn-danger delete-category" data-id="${c.id}">Удалить</button>
                            ` : ''}
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;

    document.querySelectorAll('.edit-category').forEach(b => b.addEventListener('click', () => {
        const cat = state.categories.find(x => x.id === b.dataset.id);
        openCategoryModal(cat);
    }));
    document.querySelectorAll('.delete-category').forEach(b => b.addEventListener('click', () => {
        if (confirm('Удалить?')) {
            deleteCategory(b.dataset.id).then(() => fetchCategories().then(renderCategories));
        }
    }));
}

function openCategoryModal(cat = null) {
    const title = document.getElementById('category-modal-title');
    const id = document.getElementById('category-id');
    const name = document.getElementById('category-name');
    if (cat) {
        title.textContent = 'Редактировать';
        id.value = cat.id;
        name.value = cat.name;
    } else {
        title.textContent = 'Добавить';
        id.value = '';
        name.value = '';
    }
    categoryModal.classList.add('show');
}

async function handleCategorySubmit(e) {
    e.preventDefault();
    const id = document.getElementById('category-id').value;
    const data = { name: document.getElementById('category-name').value };
    if (id) await updateCategory(id, data);
    else await createCategory(data);
    categoryModal.classList.remove('show');
    await fetchCategories();
    renderCategories();
}

async function createCategory(data) {
    const res = await api('/categories', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
    if (!res.ok) throw new Error();
}

async function updateCategory(id, data) {
    const res = await api(`/categories/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
    if (!res.ok) throw new Error();
}

async function deleteCategory(id) {
    const res = await api(`/categories/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error();
}

async function loadUsersPage() {
    if (!state.user || state.user.role !== 'Admin') return navigateTo('products');
    mainContainer.innerHTML = `
        <div class="card">
            <div class="card-header">
                <h1 class="card-title">Пользователи</h1>
                <button class="btn btn-primary" id="add-user-btn">Добавить пользователя</button>
            </div>
            <div class="card-body">
                <div class="filters-section" style="margin-bottom: 20px; padding: 15px; background: #f8f9fa; border-radius: 6px;">
                    <div style="display: grid; grid-template-columns: 1.5fr auto auto auto auto; gap: 10px; align-items: center;">
                        <div><input type="text" id="user-search-input" placeholder="Поиск по имени или email..." style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;"></div>
                        <div><select id="role-filter" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="">Все роли</option>
                            <option value="User">Пользователь</option>
                            <option value="ProUser">Про пользователь</option>
                            <option value="Admin">Администратор</option>
                        </select></div>
                        <div><select id="blocked-filter" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="">Все статусы</option>
                            <option value="active">Активные</option>
                            <option value="blocked">Заблокированные</option>
                        </select></div>
                        <div><select id="sort-users" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="name-asc">Имя А-Я</option>
                            <option value="name-desc">Имя Я-А</option>
                            <option value="email-asc">Email a-z</option>
                            <option value="email-desc">Email z-a</option>
                        </select></div>
                        <div><button class="btn btn-light" id="reset-user-filters" style="padding: 8px 16px;">Сброс</button></div>
                    </div>
                </div>
                <div id="users-container"><div class="loading">Загрузка...</div></div>
            </div>
        </div>
    `;
    document.getElementById('add-user-btn').addEventListener('click', openUserModal);
    document.getElementById('user-search-input').addEventListener('input', applyUserFilters);
    document.getElementById('role-filter').addEventListener('change', applyUserFilters);
    document.getElementById('blocked-filter').addEventListener('change', applyUserFilters);
    document.getElementById('sort-users').addEventListener('change', applyUserFilters);
    document.getElementById('reset-user-filters').addEventListener('click', resetUserFilters);

    await fetchUsers();
    applyUserFilters();
}

function applyUserFilters() {
    const searchTerm = document.getElementById('user-search-input').value.toLowerCase().trim();
    const roleFilter = document.getElementById('role-filter').value;
    const blockedFilter = document.getElementById('blocked-filter').value;
    const sortOption = document.getElementById('sort-users').value;

    let filtered = [...state.users];

    if (searchTerm) {
        filtered = filtered.filter(u => u.name.toLowerCase().includes(searchTerm) || u.email.toLowerCase().includes(searchTerm));
    }
    if (roleFilter) filtered = filtered.filter(u => u.role === roleFilter);
    if (blockedFilter === 'active') filtered = filtered.filter(u => !u.isBlocked);
    if (blockedFilter === 'blocked') filtered = filtered.filter(u => u.isBlocked);

    const [sortBy, dir] = sortOption.split('-');
    filtered.sort((a, b) => {
        const aVal = a[sortBy].toLowerCase();
        const bVal = b[sortBy].toLowerCase();
        return dir === 'asc' ? aVal.localeCompare(bVal) : bVal.localeCompare(aVal);
    });

    renderUsers(filtered);
}

function resetUserFilters() {
    document.getElementById('user-search-input').value = '';
    document.getElementById('role-filter').value = '';
    document.getElementById('blocked-filter').value = '';
    document.getElementById('sort-users').value = 'name-asc';
    renderUsers(state.users);
}

async function fetchUsers() {
    const res = await api('/users');
    if (!res.ok) throw new Error();
    state.users = await res.json();
}

function renderUsers(users = state.users) {
    const container = document.getElementById('users-container');
    if (!users.length) {
        container.innerHTML = `<div class="alert alert-info">Нет пользователей</div>`;
        return;
    }

    container.innerHTML = `
        <table class="table">
            <thead><tr><th>Email</th><th>Имя</th><th>Роль</th><th>Статус</th><th>Действия</th></tr></thead>
            <tbody>
                ${users.map(u => {
                    const canChangePwd = state.user && (u.id === state.user.id || u.role !== 'Admin');
                    const changePwdBtn = canChangePwd ? `<button class="btn btn-light change-password" data-id="${u.id}">Пароль</button>` : '';
                    const canBlock = state.user && u.role !== 'Admin';
                    const blockBtn = canBlock ? `<button class="btn btn-${u.isBlocked ? 'secondary' : 'warning'} toggle-block" data-id="${u.id}">${u.isBlocked ? 'Разблокировать' : 'Заблокировать'}</button>` : '';
                    const canDelete = state.user && (u.id === state.user.id || u.role !== 'Admin');
                    const deleteBtn = canDelete ? `<button class="btn btn-danger delete-user" data-id="${u.id}">Удалить</button>` : '';

                    return `<tr>
                        <td>${u.email}</td>
                        <td>${u.name}</td>
                        <td>${u.role}</td>
                        <td>${u.isBlocked ? 'Заблокирован' : 'Активен'}</td>
                        <td>${changePwdBtn} ${blockBtn} ${deleteBtn}</td>
                    </tr>`;
                }).join('')}
            </tbody>
        </table>
    `;

    document.querySelectorAll('.change-password').forEach(b => b.addEventListener('click', () => openChangePasswordModal(b.dataset.id)));
    document.querySelectorAll('.toggle-block').forEach(b => b.addEventListener('click', () => toggleBlockUser(b.dataset.id).then(() => fetchUsers().then(applyUserFilters))));
    document.querySelectorAll('.delete-user').forEach(b => b.addEventListener('click', () => {
        if (confirm('Удалить?')) deleteUser(b.dataset.id).then(() => fetchUsers().then(applyUserFilters));
    }));
}

function openUserModal() {
    document.getElementById('user-form').reset();
    document.getElementById('user-id').value = '';
    userModal.classList.add('show');
}

function openChangePasswordModal(id) {
    document.getElementById('change-password-user-id').value = id;
    changePasswordModal.classList.add('show');
}

async function handleUserSubmit(e) {
    e.preventDefault();
    const data = {
        email: document.getElementById('user-email').value,
        name: document.getElementById('user-name').value,
        role: document.getElementById('user-role').value,
        password: document.getElementById('user-password').value
    };

    try {
        await createUser(data);
        userModal.classList.remove('show');
        await fetchUsers();
        applyUserFilters();
    } catch (err) {
        alert(formatErrorMessage(err));
    }
}

async function handlePasswordChange(e) {
    e.preventDefault();
    const id = document.getElementById('change-password-user-id').value;
    const pwd = document.getElementById('new-password').value;

    try {
        await changePassword(id, pwd);
        changePasswordModal.classList.remove('show');
        if (id === state.user.id) {
            alert('Пароль изменён. Войдите заново.');
            await logout();
        } else {
            alert('Пароль изменён');
            await fetchUsers();
            applyUserFilters();
        }
    } catch {
        alert('Ошибка при изменении пароля');
    }
}

async function createUser(data) {
    const res = await api('/users', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
    if (!res.ok) await handleApiError(res);
    return await res.json();
}

async function changePassword(id, pwd) {
    const res = await api(`/users/${id}/password`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ newPassword: pwd }) });
    if (!res.ok) throw new Error();
}

async function toggleBlockUser(id) {
    const res = await api(`/users/${id}/block`, { method: 'PUT' });
    if (!res.ok) throw new Error();
}

async function deleteUser(id) {
    const res = await api(`/users/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error();
    if (id === state.user?.id) await logout();
}

async function loadLogsPage() {
    if (!state.user || state.user.role !== 'Admin') {
        return navigateTo('products');
    }

    mainContainer.innerHTML = `
        <div class="card">
            <div class="card-header">
                <h1 class="card-title">Логи системы в реальном времени</h1>
                <div>
                    <button class="btn btn-light" id="clear-logs">Очистить</button>
                    <button class="btn btn-secondary" id="toggle-autoscroll">Автопрокрутка: <span>вкл</span></button>
                </div>
            </div>
            <div class="card-body" style="padding: 0;">
                <div id="logs-container" style="
                    height: 70vh;
                    overflow-y: auto;
                    padding: 15px;
                    font-family: 'Courier New', monospace;
                    font-size: 0.9rem;
                    background: #1e1e1e;
                    color: #d4d4d4;
                    border-radius: 0 0 6px 6px;
                    white-space: pre-wrap;
                    word-break: break-all;
                "></div>
                <div id="connection-status" style="
                    padding: 8px 15px;
                    font-size: 0.8rem;
                    background: #333;
                    color: #aaa;
                    border-top: 1px solid #444;
                ">Подключение...</div>
            </div>
        </div>
    `;

    const logsContainer = document.getElementById('logs-container');
    const statusEl = document.getElementById('connection-status');
    const clearBtn = document.getElementById('clear-logs');
    const autoscrollBtn = document.getElementById('toggle-autoscroll');
    const autoscrollSpan = autoscrollBtn.querySelector('span');

    let eventSource = null;
    let autoscroll = true;

    autoscrollBtn.addEventListener('click', () => {
        autoscroll = !autoscroll;
        autoscrollSpan.textContent = autoscroll ? 'вкл' : 'выкл';
    });

    clearBtn.addEventListener('click', () => {
        logsContainer.innerHTML = '';
    });

    const scrollToBottom = () => {
        if (autoscroll) {
            logsContainer.scrollTop = logsContainer.scrollHeight;
        }
    };

    const addLog = (type, message) => {
        const colors = {
            create: '#4CAF50',
            update: '#FF9800',
            delete: '#F44336',
            block: '#9C27B0',
            unblock: '#2196F3',
            login: '#00BCD4',
            info: '#607D8B',
            error: '#F44336'
        };

        const color = colors[type] || '#9E9E9E';

        let timestamp = '';
        let cleanMessage = message;

        const isoDateMatch = message.match(/^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(\+\d{2}:\d{2})?)/);
        if (isoDateMatch) {
            const isoDate = isoDateMatch[1];
            const date = new Date(isoDate);

            if (!isNaN(date)) {
                timestamp = date.toLocaleString('ru-RU', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit',
                    hour12: false
                }).replace(',', '');

                cleanMessage = message.replace(isoDateMatch[0], '').trim();
            }
        }

        if (!timestamp) {
            timestamp = new Date().toLocaleString('ru-RU', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            }).replace(',', '');
        }

        const line = document.createElement('div');
        line.style.cssText = `
            padding: 2px 0;
            border-bottom: 1px solid #333;
            color: ${color};
        `;
        line.innerHTML = `<span style="color:#666; margin-right:8px;">[${timestamp}]</span> ${escapeHtml(cleanMessage)}`;
        logsContainer.appendChild(line);
        scrollToBottom();
    };

    const connect = () => {
        if (eventSource) {
            eventSource.close();
        }

        statusEl.textContent = 'Подключение...';
        statusEl.style.color = '#FF9800';

        eventSource = new EventSource(`${API_BASE}/logs/stream`, { withCredentials: true });

        eventSource.onopen = () => {
            statusEl.textContent = 'Подключено';
            statusEl.style.color = '#4CAF50';
        };

        eventSource.onmessage = (e) => { };

        eventSource.addEventListener('create', e => addLog('create', e.data));
        eventSource.addEventListener('update', e => addLog('update', e.data));
        eventSource.addEventListener('delete', e => addLog('delete', e.data));
        eventSource.addEventListener('block', e => addLog('block', e.data));
        eventSource.addEventListener('unblock', e => addLog('unblock', e.data));
        eventSource.addEventListener('login', e => addLog('login', e.data));
        eventSource.addEventListener('info', e => addLog('info', e.data));
        eventSource.addEventListener('error', e => addLog('error', e.data));

        eventSource.onerror = () => {
            statusEl.textContent = 'Ошибка подключения. Переподключение...';
            statusEl.style.color = '#F44336';
            eventSource.close();
            setTimeout(connect, 3000);
        };
    };

    connect();

    const originalNavigate = navigateTo;
    const cleanup = () => {
        if (eventSource) {
            eventSource.close();
            eventSource = null;
        }
        window.navigateTo = originalNavigate;
    };

    window.navigateTo = function(page) {
        cleanup();
        originalNavigate(page);
    };

    return cleanup;
}

function loadLoginPage() {
    if (state.user) return navigateTo('products');
    mainContainer.innerHTML = `
        <div class="card" style="max-width: 500px; margin: 40px auto;">
            <div class="card-header"><h1 class="card-title">Вход</h1></div>
            <div class="card-body">
                <form id="login-form">
                    <div class="form-group"><label>Email</label><input type="email" id="login-email" required></div>
                    <div class="form-group"><label>Пароль</label><input type="password" id="login-password" required></div>
                    <div id="login-error" class="error-message hidden" style="margin-bottom: 15px;"></div>
                    <button type="submit" class="btn btn-primary">Войти</button>
                </form>
            </div>
        </div>
    `;
    document.getElementById('login-form').addEventListener('submit', handleLogin);
}

function loadRegisterPage() {
    if (state.user) return navigateTo('products');
    mainContainer.innerHTML = `
        <div class="card" style="max-width: 500px; margin: 40px auto;">
            <div class="card-header"><h1 class="card-title">Регистрация</h1></div>
            <div class="card-body">
                <form id="register-form">
                    <div class="form-group"><label>Email</label><input type="email" id="register-email" required></div>
                    <div class="form-group"><label>Имя</label><input type="text" id="register-name" required></div>
                    <div class="form-group"><label>Пароль</label>
                        <input type="password" id="register-password" minlength="8" maxlength="20" required>
                    </div>
                    <div id="register-error" class="error-message hidden" style="margin-bottom: 15px;"></div>
                    <button type="submit" class="btn btn-primary">Зарегистрироваться</button>
                </form>
            </div>
        </div>
    `;
    document.getElementById('register-form').addEventListener('submit', handleRegister);
}

async function handleLogin(e) {
    e.preventDefault();
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const err = document.getElementById('login-error');
    try {
        const res = await api('/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        if (res.ok) {
            await checkAuth();
            navigateTo('products');
        } else {
            err.textContent = 'Неверный email или пароль';
            err.classList.remove('hidden');
        }
    } catch {
        err.textContent = 'Ошибка';
        err.classList.remove('hidden');
    }
}

async function handleRegister(e) {
    e.preventDefault();
    const email = document.getElementById('register-email').value;
    const name = document.getElementById('register-name').value;
    const password = document.getElementById('register-password').value;
    const err = document.getElementById('register-error');

    try {
        const res = await api('/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, name, password })
        });

        if (res.ok) {
            alert('Успешно! Войдите.');
            navigateTo('login');
        } else {
            let errorData;
            try { errorData = await res.json(); } catch { errorData = { message: await res.text() }; }
            let message = 'Неизвестная ошибка';
            if (res.status === 409) message = 'Пользователь с таким email уже существует';
            else if (res.status === 400 && errorData?.errors) {
                message = `Ошибки валидации:\n${Object.values(errorData.errors).flat().join('.\n')}`;
            } else if (errorData?.error) message = errorData.error;
            else if (errorData?.title) message = errorData.title;
            else if (errorData?.detail) message = errorData.detail;
            else if (errorData?.message) message = errorData.message;
            else message = `Ошибка ${res.status}`;

            err.textContent = message;
            err.classList.remove('hidden');
        }
    } catch {
        err.textContent = 'Ошибка сети';
        err.classList.remove('hidden');
    }
}

async function logout() {
    await api('/auth/logout', { method: 'POST' });
    state.user = null;
    userInfo.classList.add('hidden');
    loginBtn.classList.remove('hidden');
    registerBtn.classList.remove('hidden');
    updateNavigationVisibility();
    navigateTo('products');
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

init();