const API_BASE_URL = 'http://localhost:5143/api/v1'; // Asegúrate de que esta URL sea correcta

// --- Utilidades Generales ---
function showAlert(message, type = 'success') {
    const alertContainer = document.getElementById('alertContainer') || (() => {
        const div = document.createElement('div');
        div.id = 'alertContainer';
        div.className = 'alert-container';
        document.body.appendChild(div);
        return div;
    })();

    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    alertContainer.appendChild(alertDiv);

    setTimeout(() => {
        bootstrap.Alert.getInstance(alertDiv)?.close();
    }, 5000);
}

async function fetchData(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
        },
    };
    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(url, options);
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `Error: ${response.status} ${response.statusText}`);
        }
        // Algunas respuestas pueden no tener cuerpo (ej. 204 No Content)
        if (response.status === 204) {
            return null;
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching data:', error);
        showAlert(error.message || 'Ocurrió un error en la solicitud.', 'danger');
        throw error; // Re-throw to allow specific error handling in calling functions
    }
}

// --- Lógica para Productos (productos.html) ---
async function loadProducts(page = 1, pageSize = 10, name = '', category = '', active = '') {
    const productList = document.getElementById('productList');
    const pagination = document.getElementById('pagination');
    if (!productList || !pagination) return; // Exit if not on the products page

    productList.innerHTML = '<tr><td colspan="7">Cargando productos...</td></tr>';
    pagination.innerHTML = '';

    let url = `${API_BASE_URL}/Productos?page=${page}&pageSize=${pageSize}`;
    if (name) url += `&nombre=${name}`;
    if (category) url += `&categoria=${category}`;
    if (active !== '') url += `&activo=${active}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `Error: ${response.status} ${response.statusText}`);
        }

        const totalItems = parseInt(response.headers.get('X-Total-Count') || '0');
        const products = await response.json();

        productList.innerHTML = '';
        if (products.length === 0) {
            productList.innerHTML = '<tr><td colspan="7">No hay productos disponibles.</td></tr>';
            return;
        }

        products.forEach(product => {
            const row = productList.insertRow();
            row.insertCell().textContent = product.id;
            row.insertCell().textContent = product.nombre;
            row.insertCell().textContent = product.categoria;
            row.insertCell().textContent = product.precio.toFixed(2);
            row.insertCell().textContent = product.stock;
            row.insertCell().textContent = product.activo ? 'Sí' : 'No';

            const actionsCell = row.insertCell();
            const editBtn = document.createElement('button');
            editBtn.className = 'btn btn-sm btn-warning me-2';
            editBtn.textContent = 'Editar';
            editBtn.onclick = () => editProduct(product);
            actionsCell.appendChild(editBtn);

            const deleteBtn = document.createElement('button');
            deleteBtn.className = 'btn btn-sm btn-danger';
            deleteBtn.textContent = 'Eliminar';
            deleteBtn.onclick = () => deleteProduct(product.id);
            actionsCell.appendChild(deleteBtn);
        });

        renderPagination(totalItems, page, pageSize, (newPage) => {
            const filterName = document.getElementById('filterName').value;
            const filterCategory = document.getElementById('filterCategory').value;
            const filterActive = document.getElementById('filterActive').value;
            loadProducts(newPage, pageSize, filterName, filterCategory, filterActive);
        });

    } catch (error) {
        console.error('Error loading products:', error);
        productList.innerHTML = `<tr><td colspan="7" class="text-danger">Error al cargar productos: ${error.message}</td></tr>`;
    }
}

function renderPagination(totalItems, currentPage, pageSize, pageChangeCallback) {
    const totalPages = Math.ceil(totalItems / pageSize);
    const pagination = document.getElementById('pagination');
    pagination.innerHTML = '';

    if (totalPages <= 1) return;

    const createPageItem = (pageNumber, text, isActive = false, isDisabled = false) => {
        const li = document.createElement('li');
        li.className = `page-item ${isActive ? 'active' : ''} ${isDisabled ? 'disabled' : ''}`;
        const a = document.createElement('a');
        a.className = 'page-link';
        a.href = '#';
        a.textContent = text;
        if (!isDisabled && !isActive) {
            a.onclick = (e) => {
                e.preventDefault();
                pageChangeCallback(pageNumber);
            };
        }
        li.appendChild(a);
        return li;
    };

    pagination.appendChild(createPageItem(currentPage - 1, 'Anterior', false, currentPage === 1));

    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, currentPage + 2);

    if (startPage > 1) {
        pagination.appendChild(createPageItem(1, '1'));
        if (startPage > 2) {
            const ellipsis = document.createElement('li');
            ellipsis.className = 'page-item disabled';
            ellipsis.innerHTML = '<span class="page-link">...</span>';
            pagination.appendChild(ellipsis);
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        pagination.appendChild(createPageItem(i, i.toString(), i === currentPage));
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            const ellipsis = document.createElement('li');
            ellipsis.className = 'page-item disabled';
            ellipsis.innerHTML = '<span class="page-link">...</span>';
            pagination.appendChild(ellipsis);
        }
        pagination.appendChild(createPageItem(totalPages, totalPages.toString()));
    }

    pagination.appendChild(createPageItem(currentPage + 1, 'Siguiente', false, currentPage === totalPages));
}


async function getProductById(id) {
    try {
        const product = await fetchData(`${API_BASE_URL}/Productos/${id}`);
        showAlert(`Producto encontrado: ${product.nombre}`, 'info');
        // Puedes mostrar los detalles en algún lugar de la página si lo deseas
        console.log('Detalles del producto:', product);
    } catch (error) {
        // Error handled by fetchData
    }
}

async function createOrUpdateProduct(event) {
    event.preventDefault();
    const productId = document.getElementById('productId').value;
    const product = {
        nombre: document.getElementById('productName').value,
        categoria: document.getElementById('productCategory').value,
        descripcion: document.getElementById('productDescription').value,
        precio: parseFloat(document.getElementById('productPrice').value),
        stock: parseInt(document.getElementById('productStock').value),
        activo: document.getElementById('productActive').checked
    };

    try {
        if (productId) {
            await fetchData(`${API_BASE_URL}/Productos/${productId}`, 'PUT', product);
            showAlert('Producto actualizado exitosamente.', 'success');
        } else {
            await fetchData(`${API_BASE_URL}/Productos`, 'POST', product);
            showAlert('Producto creado exitosamente.', 'success');
        }
        clearProductForm();
        loadProducts();
    } catch (error) {
        // Error handled by fetchData
    }
}

function editProduct(product) {
    document.getElementById('productId').value = product.id;
    document.getElementById('productName').value = product.nombre;
    document.getElementById('productCategory').value = product.categoria;
    document.getElementById('productDescription').value = product.descripcion;
    document.getElementById('productPrice').value = product.precio;
    document.getElementById('productStock').value = product.stock;
    document.getElementById('productActive').checked = product.activo;
    window.scrollTo(0, 0); // Scroll to top to see the form
}

async function deleteProduct(id) {
    if (!confirm('¿Estás seguro de que quieres eliminar este producto?')) return;
    try {
        await fetchData(`${API_BASE_URL}/Productos/${id}`, 'DELETE');
        showAlert('Producto eliminado exitosamente.', 'success');
        loadProducts();
    } catch (error) {
        // Error handled by fetchData
    }
}

async function loadLowStockProducts() {
    const productList = document.getElementById('productList');
    const pagination = document.getElementById('pagination');
    if (!productList || !pagination) return;

    productList.innerHTML = '<tr><td colspan="7">Cargando productos con stock bajo...</td></tr>';
    pagination.innerHTML = '';

    try {
        const response = await fetch(`${API_BASE_URL}/Productos/low-stock`);
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `Error: ${response.status} ${response.statusText}`);
        }
        const products = await response.json();

        productList.innerHTML = '';
        if (products.length === 0) {
            productList.innerHTML = '<tr><td colspan="7">No hay productos con stock bajo.</td></tr>';
            return;
        }

        products.forEach(product => {
            const row = productList.insertRow();
            row.insertCell().textContent = product.id;
            row.insertCell().textContent = product.nombre;
            row.insertCell().textContent = product.categoria;
            row.insertCell().textContent = product.precio.toFixed(2);
            row.insertCell().textContent = product.stock;
            row.insertCell().textContent = product.activo ? 'Sí' : 'No';

            const actionsCell = row.insertCell();
            const editBtn = document.createElement('button');
            editBtn.className = 'btn btn-sm btn-warning me-2';
            editBtn.textContent = 'Editar';
            editBtn.onclick = () => editProduct(product);
            actionsCell.appendChild(editBtn);

            const deleteBtn = document.createElement('button');
            deleteBtn.className = 'btn btn-sm btn-danger';
            deleteBtn.textContent = 'Eliminar';
            deleteBtn.onclick = () => deleteProduct(product.id);
            actionsCell.appendChild(deleteBtn);
        });
        showAlert('Productos con stock bajo cargados.', 'info');
    } catch (error) {
        console.error('Error loading low stock products:', error);
        productList.innerHTML = `<tr><td colspan="7" class="text-danger">Error al cargar productos con stock bajo: ${error.message}</td></tr>`;
    }
}

function clearProductForm() {
    document.getElementById('productId').value = '';
    document.getElementById('productName').value = '';
    document.getElementById('productCategory').value = '';
    document.getElementById('productDescription').value = '';
    document.getElementById('productPrice').value = '';
    document.getElementById('productStock').value = '';
    document.getElementById('productActive').checked = true;
}

// --- Lógica para Clientes (clientes.html) ---
async function createOrUpdateClient(event) {
    event.preventDefault();
    const clientId = document.getElementById('clientId').value;
    const client = {
        nombre: document.getElementById('clientName').value,
        email: document.getElementById('clientEmail').value,
        direccion: document.getElementById('clientAddress').value,
        telefono: document.getElementById('clientPhone').value,
        activo: document.getElementById('clientActive').checked
    };

    try {
        if (clientId) {
            await fetchData(`${API_BASE_URL}/Clientes/${clientId}`, 'PUT', client);
            showAlert('Información del cliente actualizada exitosamente.', 'success');
        } else {
            const newClient = await fetchData(`${API_BASE_URL}/Clientes`, 'POST', client);
            showAlert(`Cliente ${newClient.nombre} registrado exitosamente con ID: ${newClient.id}.`, 'success');
        }
        clearClientForm();
    } catch (error) {
        // Error handled by fetchData
    }
}

async function searchClient() {
    const clientId = document.getElementById('searchClientId').value;
    const clientDetailsDiv = document.getElementById('clientDetails');
    const clientOrdersCard = document.getElementById('clientOrdersCard');
    const clientOrdersList = document.getElementById('clientOrdersList');
    const clientOrdersName = document.getElementById('clientOrdersName');

    clientDetailsDiv.innerHTML = '';
    clientOrdersCard.style.display = 'none';
    clientOrdersList.innerHTML = '';

    if (!clientId) {
        showAlert('Por favor, introduce un ID de cliente.', 'warning');
        return;
    }

    try {
        const client = await fetchData(`${API_BASE_URL}/Clientes/${clientId}`);
        clientDetailsDiv.innerHTML = `
            <div class="card card-body bg-light">
                <h5>Detalles del Cliente (ID: ${client.id})</h5>
                <p><strong>Nombre:</strong> ${client.nombre}</p>
                <p><strong>Email:</strong> ${client.email}</p>
                <p><strong>Dirección:</strong> ${client.direccion}</p>
                <p><strong>Teléfono:</strong> ${client.telefono}</p>
                <p><strong>Activo:</strong> ${client.activo ? 'Sí' : 'No'}</p>
                <button class="btn btn-sm btn-warning" onclick="editClient(${client.id}, '${client.nombre}', '${client.email}', '${client.direccion}', '${client.telefono}', ${client.activo})">Editar este Cliente</button>
                <button class="btn btn-sm btn-info mt-2" onclick="loadClientOrders(${client.id}, '${client.nombre}')">Ver Historial de Pedidos</button>
            </div>
        `;
        showAlert(`Cliente ${client.nombre} encontrado.`, 'info');
    } catch (error) {
        clientDetailsDiv.innerHTML = `<div class="alert alert-danger">Cliente no encontrado o inactivo.</div>`;
        // Error handled by fetchData
    }
}

function editClient(id, name, email, address, phone, active) {
    document.getElementById('clientId').value = id;
    document.getElementById('clientName').value = name;
    document.getElementById('clientEmail').value = email;
    document.getElementById('clientAddress').value = address;
    document.getElementById('clientPhone').value = phone;
    document.getElementById('clientActive').checked = active;
    window.scrollTo(0, 0); // Scroll to top to see the form
}

async function loadClientOrders(clientId, clientName) {
    const clientOrdersList = document.getElementById('clientOrdersList');
    const clientOrdersCard = document.getElementById('clientOrdersCard');
    const clientOrdersName = document.getElementById('clientOrdersName');

    clientOrdersList.innerHTML = '<tr><td colspan="4">Cargando historial de pedidos...</td></tr>';
    clientOrdersCard.style.display = 'block';
    clientOrdersName.textContent = clientName;

    try {
        const orders = await fetchData(`${API_BASE_URL}/Clientes/${clientId}/pedidos`);
        clientOrdersList.innerHTML = '';
        if (orders.length === 0) {
            clientOrdersList.innerHTML = '<tr><td colspan="4">No hay pedidos para este cliente.</td></tr>';
            return;
        }
        orders.forEach(order => {
            const row = clientOrdersList.insertRow();
            row.insertCell().textContent = order.id;
            row.insertCell().textContent = new Date(order.fechaPedido).toLocaleDateString();
            row.insertCell().textContent = order.estado;
            row.insertCell().textContent = order.direccionEnvio;
        });
        showAlert(`Historial de pedidos para ${clientName} cargado.`, 'info');
    } catch (error) {
        clientOrdersList.innerHTML = `<tr class="text-danger"><td colspan="4">Error al cargar pedidos: ${error.message}</td></tr>`;
        // Error handled by fetchData
    }
}

function clearClientForm() {
    document.getElementById('clientId').value = '';
    document.getElementById('clientName').value = '';
    document.getElementById('clientEmail').value = '';
    document.getElementById('clientAddress').value = '';
    document.getElementById('clientPhone').value = '';
    document.getElementById('clientActive').checked = true;
}

// --- Lógica para Pedidos (pedidos.html) ---
let currentOrderIdForStatusUpdate = null;

async function createOrder(event) {
    event.preventDefault();
    const order = {
        clienteId: parseInt(document.getElementById('orderClientId').value),
        direccionEnvio: document.getElementById('orderShippingAddress').value,
        fechaPedido: new Date().toISOString(), // Backend will set this, but good to send
        estado: "Pendiente" // Default state
    };

    try {
        const newOrder = await fetchData(`${API_BASE_URL}/Pedidos`, 'POST', order);
        showAlert(`Pedido creado exitosamente con ID: ${newOrder.id}.`, 'success');
        document.getElementById('orderForm').reset();
    } catch (error) {
        // Error handled by fetchData
    }
}

async function searchOrder() {
    const orderId = document.getElementById('searchOrderId').value;
    const orderDetailsDiv = document.getElementById('orderDetails');
    const updateOrderStatusCard = document.getElementById('updateOrderStatusCard');
    const currentOrderIdSpan = document.getElementById('currentOrderId');
    const newOrderStatusSelect = document.getElementById('newOrderStatus');

    orderDetailsDiv.innerHTML = '';
    updateOrderStatusCard.style.display = 'none';
    currentOrderIdForStatusUpdate = null;

    if (!orderId) {
        showAlert('Por favor, introduce un ID de pedido.', 'warning');
        return;
    }

    try {
        const order = await fetchData(`${API_BASE_URL}/Pedidos/${orderId}`);
        orderDetailsDiv.innerHTML = `
            <div class="card card-body bg-light">
                <h5>Detalles del Pedido (ID: ${order.id})</h5>
                <p><strong>Cliente ID:</strong> ${order.clienteId}</p>
                <p><strong>Fecha Pedido:</strong> ${new Date(order.fechaPedido).toLocaleDateString()}</p>
                <p><strong>Estado:</strong> <span id="displayOrderStatus">${order.estado}</span></p>
                <p><strong>Dirección de Envío:</strong> ${order.direccionEnvio}</p>
            </div>
        `;
        showAlert(`Pedido ${order.id} encontrado.`, 'info');

        currentOrderIdForStatusUpdate = order.id;
        currentOrderIdSpan.textContent = order.id;
        newOrderStatusSelect.value = order.estado;
        updateOrderStatusCard.style.display = 'block';

    } catch (error) {
        orderDetailsDiv.innerHTML = `<div class="alert alert-danger">Pedido no encontrado.</div>`;
        // Error handled by fetchData
    }
}

async function updateOrderStatus() {
    if (!currentOrderIdForStatusUpdate) {
        showAlert('No hay un pedido seleccionado para actualizar.', 'warning');
        return;
    }
    const newStatus = document.getElementById('newOrderStatus').value;

    try {
        await fetchData(`${API_BASE_URL}/Pedidos/${currentOrderIdForStatusUpdate}/estado`, 'PATCH', newStatus);
        showAlert(`Estado del pedido ${currentOrderIdForStatusUpdate} actualizado a "${newStatus}".`, 'success');
        document.getElementById('displayOrderStatus').textContent = newStatus; // Update displayed status
    } catch (error) {
        // Error handled by fetchData
    }
}

async function cancelOrder() {
    if (!currentOrderIdForStatusUpdate) {
        showAlert('No hay un pedido seleccionado para cancelar.', 'warning');
        return;
    }
    if (!confirm(`¿Estás seguro de que quieres cancelar el pedido ${currentOrderIdForStatusUpdate}?`)) return;

    try {
        await fetchData(`${API_BASE_URL}/Pedidos/${currentOrderIdForStatusUpdate}`, 'DELETE');
        showAlert(`Pedido ${currentOrderIdForStatusUpdate} cancelado.`, 'success');
        document.getElementById('displayOrderStatus').textContent = 'Cancelado'; // Update displayed status
        document.getElementById('updateOrderStatusCard').style.display = 'none'; // Hide update section
        currentOrderIdForStatusUpdate = null;
    } catch (error) {
        // Error handled by fetchData
    }
}

// --- Lógica para Prescripciones (prescripciones.html) ---
let currentPrescriptionIdForValidation = null;

async function uploadPrescription(event) {
    event.preventDefault();
    const prescription = {
        nombreArchivo: document.getElementById('prescriptionFileName').value,
        clienteId: parseInt(document.getElementById('prescriptionClientId').value),
        pedidoId: document.getElementById('prescriptionOrderId').value ? parseInt(document.getElementById('prescriptionOrderId').value) : 0, // 0 if empty
        validada: false // Default state
    };

    try {
        const newPrescription = await fetchData(`${API_BASE_URL}/Prescripciones`, 'POST', prescription);
        showAlert(`Prescripción subida exitosamente con ID: ${newPrescription.id}.`, 'success');
        document.getElementById('prescriptionForm').reset();
    } catch (error) {
        // Error handled by fetchData
    }
}

async function searchPrescription() {
    const prescriptionId = document.getElementById('searchPrescriptionId').value;
    const prescriptionDetailsDiv = document.getElementById('prescriptionDetails');
    const validatePrescriptionSection = document.getElementById('validatePrescriptionSection');

    prescriptionDetailsDiv.innerHTML = '';
    validatePrescriptionSection.style.display = 'none';
    currentPrescriptionIdForValidation = null;

    if (!prescriptionId) {
        showAlert('Por favor, introduce un ID de prescripción.', 'warning');
        return;
    }

    try {
        const prescription = await fetchData(`${API_BASE_URL}/Prescripciones/${prescriptionId}`);
        prescriptionDetailsDiv.innerHTML = `
            <div class="card card-body bg-light">
                <h5>Detalles de la Prescripción (ID: ${prescription.id})</h5>
                <p><strong>Nombre Archivo:</strong> ${prescription.nombreArchivo}</p>
                <p><strong>Cliente ID:</strong> ${prescription.clienteId}</p>
                <p><strong>Pedido ID:</strong> ${prescription.pedidoId || 'N/A'}</p>
                <p><strong>Validada:</strong> <span id="displayPrescriptionValidated">${prescription.validada ? 'Sí' : 'No'}</span></p>
            </div>
        `;
        showAlert(`Prescripción ${prescription.id} encontrada.`, 'info');

        if (!prescription.validada) {
            currentPrescriptionIdForValidation = prescription.id;
            validatePrescriptionSection.style.display = 'block';
        }

    } catch (error) {
        prescriptionDetailsDiv.innerHTML = `<div class="alert alert-danger">Prescripción no encontrada.</div>`;
        // Error handled by fetchData
    }
}

async function validatePrescription() {
    if (!currentPrescriptionIdForValidation) {
        showAlert('No hay una prescripción seleccionada para validar.', 'warning');
        return;
    }
    if (!confirm(`¿Estás seguro de que quieres validar la prescripción ${currentPrescriptionIdForValidation}?`)) return;

    try {
        await fetchData(`${API_BASE_URL}/Prescripciones/${currentPrescriptionIdForValidation}/validar`, 'PATCH', true); // PATCH expects a body, sending true
        showAlert(`Prescripción ${currentPrescriptionIdForValidation} validada exitosamente.`, 'success');
        document.getElementById('displayPrescriptionValidated').textContent = 'Sí'; // Update displayed status
        document.getElementById('validatePrescriptionSection').style.display = 'none'; // Hide validate button
        currentPrescriptionIdForValidation = null;
    } catch (error) {
        // Error handled by fetchData
    }
}


// --- Inicialización de Event Listeners ---
document.addEventListener('DOMContentLoaded', () => {
    // Add a global alert container
    const alertContainer = document.createElement('div');
    alertContainer.id = 'alertContainer';
    alertContainer.className = 'alert-container';
    document.body.appendChild(alertContainer);

    // Productos Page
    if (window.location.pathname.includes('productos.html')) {
        loadProducts();
        document.getElementById('productForm').addEventListener('submit', createOrUpdateProduct);
        document.getElementById('clearFormBtn').addEventListener('click', clearProductForm);
        document.getElementById('applyFiltersBtn').addEventListener('click', () => {
            const filterName = document.getElementById('filterName').value;
            const filterCategory = document.getElementById('filterCategory').value;
            const filterActive = document.getElementById('filterActive').value;
            loadProducts(1, 10, filterName, filterCategory, filterActive);
        });
        document.getElementById('lowStockBtn').addEventListener('click', loadLowStockProducts);
    }

    // Clientes Page
    if (window.location.pathname.includes('clientes.html')) {
        document.getElementById('clientForm').addEventListener('submit', createOrUpdateClient);
        document.getElementById('clearClientFormBtn').addEventListener('click', clearClientForm);
        document.getElementById('searchClientBtn').addEventListener('click', searchClient);
    }

    // Pedidos Page
    if (window.location.pathname.includes('pedidos.html')) {
        document.getElementById('orderForm').addEventListener('submit', createOrder);
        document.getElementById('searchOrderBtn').addEventListener('click', searchOrder);
        document.getElementById('updateStatusBtn').addEventListener('click', updateOrderStatus);
        document.getElementById('cancelOrderBtn').addEventListener('click', cancelOrder);
    }

    // Prescripciones Page
    if (window.location.pathname.includes('prescripciones.html')) {
        document.getElementById('prescriptionForm').addEventListener('submit', uploadPrescription);
        document.getElementById('searchPrescriptionBtn').addEventListener('click', searchPrescription);
        document.getElementById('validatePrescriptionBtn').addEventListener('click', validatePrescription);
    }
});