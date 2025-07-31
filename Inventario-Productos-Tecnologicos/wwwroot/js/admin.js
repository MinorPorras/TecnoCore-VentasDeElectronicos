const searchProductDialog = $('#searchProductDialog');
const searchProductDialogContent = searchProductDialog.find('.searchProductDialogContent');
const searchProductTblContent = searchProductDialog.find('.tbl-content');
const searchProductInput = $('#searchProductInput');
const btnSearchProduct = $('#btnSearchProduct');
const inputProductId = $('#ProductoId');
const inputProductName = $('#ProductoNombre');
const inputProductStock = $('#StockAnterior');

document.addEventListener("DOMContentLoaded", function () {
    $(document).ready(() => {
        if (document.querySelector('.modifyElement')) {
            if (document.querySelector('.EditProduct')) {
                modifyProduct();
            }else{
                modifyElement();
            }
            if (document.querySelector('#imgSelector')) {
                let imgForm = document.querySelector('#imgSelector');
                imgForm.addEventListener('change', () => mostrarImagen(imgForm));
            }
        }
        if (document.querySelector('.deleteDialog')) {
            deleteElement();
        }
        
        if (document.querySelector('.kardexForm')) {
            $('body').on('click', '#btnExitModalSearch', () => {
                closeModalAnimation(searchProductDialogContent[0], searchProductDialog[0]);
            });
            
            kardexHandlers();
            initSearchProductModal();
        }
        if (document.querySelector('.cuponesForm')) {
            handleCuponesForm(true);
        }
        if (document.querySelector('.cuponesFormEdit')) {
            handleCuponesForm(false);
        }
    });
});

function mostrarImagen(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();
        reader.onload = function (e) {
            let preview = document.getElementById('preview');
            preview.src = e.target.result;
            preview.style.display = 'block';
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function showAlert(message, type = 'success') {
    const alertContainer = document.getElementById('alertContainer');
    const alert = document.createElement('div');
    console.log(type)
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.role = 'alert';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    alertContainer.appendChild(alert);

    // Auto-cerrar la alerta después de 5 segundos
    setTimeout(() => {
        alert.classList.remove('show');
        setTimeout(() => alert.remove(), 150);
    }, 5000);
}

function modifyProduct(){
    const updateBtn = document.getElementById('updateBtn');
    updateBtn.addEventListener('click', async (e) => {
        e.preventDefault();
        const form = document.querySelector('.modifyElement'); // Referencia al formulario
        const controller = document.getElementById('controller').value;
        const action = document.getElementById('action').value;
        
        const formData = new FormData(form);

        // Para el `TB_Novedad` (checkbox)
        const novedadCheckbox = form.querySelector('input[name="TB_Novedad"]');
        if (novedadCheckbox) {
            formData.set('TB_Novedad', novedadCheckbox.checked); // Establece true/false
        }
        
        try {
            console.log(`Enviando la información del form de actualización`);
            for (let pair of formData.entries()) {
                console.log(`${pair[0]}: ${pair[1]}`);
            }
            
            const response = await fetch(`/${controller}/${action}`, {
                method: 'PUT',
                body: formData
            });
            if (response.ok) {
                if (response.status === 204) {
                    console.log('No hay contenido por mostrar, se asume que la actualización fue exitosa');
                } else {
                    console.log('Elemento modificado correctamente');
                }
                window.history.back();
                window.location.reload();
            } else {
                const errorText = await response.text();
                console.error('Error al modificar el elemento:', errorText);
                showAlert('Error al modificar el elemento: ' + errorText, 'danger');
            }
        }catch(err) {
            console.error('Error al modificar el elemento: ' + err, 'danger');
        }
    })
}

function modifyElement() {
    const updateBtn = document.getElementById('updateBtn')
    updateBtn.addEventListener('click', async (e) => {
        e.preventDefault()
        const form = document.querySelector('.modifyElement');
        const controller = document.getElementById('controller').value
        const action = document.getElementById('action').value
        const values = {};
        
        console.log("Iniciando evento de ");

        form.querySelectorAll('input[name], select[name], textarea[name]').forEach(el => {
            console.log(`Nombre del elemento: ${el.name}`)
            // Ignorar ciertos campos
            if (el.name === 'controller' || el.name === '__RequestVerificationToken' || el.name === 'action') {
                return;
            }

            // Manejar diferentes tipos de inputs
            if (el.type === 'checkbox') {
                values[el.name] = el.checked;
            } else if (el.type === 'radio') {
                if (el.checked) {
                    values[el.name] = el.value === 'true';
                }
            } else {
                // Manejar casos especiales
                switch (el.name) {
                    case 'TN_Id':
                    case 'TN_CategoriaId':
                    case 'TN_MarcaId':
                        values[el.name] = parseInt(el.value);
                        break;
                    case 'TB_Activo':
                        console.log('Activo:', el.value);
                        values[el.name] = el.value === "true";
                        break;
                    default:
                        values[el.name] = el.value;
                }
            }
            console.log(`${el.name}: ${values[el.name]}`);
        });

        try {
            console.log(values)
            const bodyRequest = JSON.stringify(values);
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const response = await fetch(`/${controller}/${action}`, {
                method: "PUT",
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: bodyRequest
            })

            // Check if the response status is 204 No Content
            if (response.status === 204) {
                return;
            }

            if (response.ok) {
                console.log('Elemento modificado correctamente');
                if (action !== 'EditSubcategoria') {
                    window.location.href = `/${controller}/Index`;
                } else {
                    window.history.back();
                    window.location.reload();
                }
            }
        } catch (e) {
            console.error('Error:', e);
        }
    })
}

function deleteElement() {
    const showModalBtns = document.querySelectorAll('.showModal');
    const deleteDialog = document.querySelector('.deleteDialog');
    const dialogContent = deleteDialog.querySelector('.dialogContent');
    const btnCancel = document.querySelector('#btnCancel');
    const btnSubmit = document.querySelector('#btnSubmit');

    if (!deleteDialog || !showModalBtns.length) return;

    showModalBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-id');
            const name = btn.getAttribute('value');
            const isActive = btn.getAttribute('data-active').toLowerCase();
            const idInput = deleteDialog.querySelector('#idDelete');
            const activeInput = deleteDialog.querySelector('#active');

            // Establecer valores en el formulario
            idInput.value = id;
            activeInput.value = !isActive; // Invertimos el valor actual

            // Actualizar el título y el botón del diálogo
            const dialogTitle = deleteDialog.querySelector('h1');
            let accion;
            switch (isActive) {
                case 'true':
                    accion = 'desactivar';
                    btnSubmit.classList.add('btnDeactivate');
                    btnSubmit.classList.remove('btnActivate');
                    btnSubmit.textContent = 'Desactivar';
                    break;
                case 'false':
                    accion = 'activar';
                    btnSubmit.classList.add('btnActivate');
                    btnSubmit.classList.remove('btnDeactivate');
                    btnSubmit.textContent = 'Activar';
                    break;
                case 'delete':
                    accion = 'eliminar';
                    btnSubmit.classList.add('btnDeactivate');
                    btnSubmit.classList.remove('btnActivate');
                    btnSubmit.textContent = 'Eliminar';
                    break;
                default:
                    accion = 'realizar esta acción sobre';
                    btnSubmit.classList.add('btnActivate');
                    btnSubmit.classList.remove('btnDeactivate');
                    btnSubmit.textContent = 'Confirmar';
                    break;
            }
            dialogTitle.textContent = `¿Desea ${accion}: ${name}?`;

            deleteDialog.showModal();
        });
    });

    // Manejar el cierre del diálogo con el botón Cancelar
    btnCancel.addEventListener('click', () => {
        deleteDialog.close();
    });

    // Cerrar el diálogo al hacer clic fuera de él
    dialogContent.addEventListener('click', (e) => {
        const dialogDimensions = dialogContent.getBoundingClientRect();
        if (
            e.clientX < dialogDimensions.left ||
            e.clientX > dialogDimensions.right ||
            e.clientY < dialogDimensions.top ||
            e.clientY > dialogDimensions.bottom
        ) {
            dialogContent.close();
        }
    });
}

function initSearchProductModal(){

    // Se carga el evento del doble click sobre el input donde se ingresan los código de los productos
    $('#ProductoNombre').on('dblclick', (e) => {
        e.stopPropagation();
        showModal(searchProductDialog[0]);
        loadProductsForSearchModal('');
    });

    btnSearchProduct.on('click', () => {
        const searchTerm = searchProductInput.val();
        loadProductsForSearchModal(searchTerm);
    });

    searchProductInput.on('keypress', function(e) {
        if (e.which === 13) { // 13 es el código para la tecla Enter
            btnSearchProduct.click(); // Simula un clic en el botón de búsqueda
        }
    });

    $('body').on('click', '.selectSearchProductBtn', function() {
        const productId = $(this).data('productid'); // 'this' ahora se refiere al botón clicado
        const productName = $(this).data('productname');
        const productStock = $(this).data('productstock');

        console.log("Producto ID seleccionado:", productId); // Para depuración
        console.log("Nombre seleccionado:", productName); // Para depuración
        console.log("Stock seleccionado:", productStock); // Para depuración

        if (!isNaN(parseInt(productId))) {
            
            inputProductId.val(productId).text(productId);
            inputProductName.val(productName).text(productName);
            inputProductStock.val(productStock).text(productStock);
            
            closeModalAnimation(searchProductDialogContent[0], searchProductDialog[0]);
        } else {
            console.error("Error: Product ID no es válido o no se pudo obtener.");
            showAlert('Error: No se pudo seleccionar el producto. Intente de nuevo.', 'error');
        }
    });
}
function loadProductsForSearchModal(searchTerm = '') { // Parámetro con valor por defecto
    searchProductTblContent.empty().append('<div class="tableRow"><span class="NoElements">Cargando productos...</span></div>');

    // Construir la URL con el parámetro de búsqueda
    const url = `/Caja/GetProductToSearch?searchTerm=${encodeURIComponent(searchTerm)}`;

    fetch(url, {
        method: 'GET',
        headers: { 'Accept': 'application/json' }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            searchProductTblContent.empty(); // Limpiar el mensaje de carga

            if (data && data.length > 0) {
                data.forEach(prod => {
                    const productRow = `
                    <div class="tableRow tSearchProduct">
                        <span class="tableCell">${prod.tn_Id}</span>
                        <span class="tableCell">${prod.tc_Nombre}</span>
                        <span class="tableCell centerTextCell">${prod.tn_Stock}</span>
                        <div class="tableButtonsColumn">
                            <button type="button" data-productId="${prod.tn_Id}" data-productName="${prod.tc_Nombre}"
                             data-productStock="${prod.tn_Stock}" class="selectSearchProductBtn btnCreate tooltipContainer">
                                <img src="/img/ICO_Add.svg" alt="Seleccionar"/>
                                <span class="TooltipText">Seleccionar</span>
                            </button>
                        </div>
                    </div>
                `;
                    searchProductTblContent.append(productRow);
                });
                searchProductTblContent.append('<div class="tableRow listEnd"><span>Fin de la Lista</span></div>');
            } else {
                searchProductTblContent.append('<div class="tableRow"><span class="NoElements">No se encontraron productos.</span></div>');
            }
        })
        .catch(error => {
            console.error('Error al cargar productos para el modal de búsqueda:', error);
            searchProductTblContent.empty().append('<div class="tableRow"><span class="NoElements">Error al cargar productos. Por favor, inténtelo de nuevo.</span></div>');
            showAlert('Error al cargar los productos para la búsqueda.', 'error');
        });
}


function closeModalAnimation(modalContent, Modal) {
    console.log("Cerrando Modal")
    console.log(Modal);
    if (!modalContent) {
        return;
    }
    if (!modalContent.classList.contains('modal-fade-out')) {
        modalContent.classList.add('modal-fade-out');
        modalContent.addEventListener('animationend', () => {
            modalContent.classList.remove('modal-fade-out');
            Modal.close();
        }, {once: true});
    }
}

function showModal(modal) {
    if (modal && modal.showModal) {
        modal.showModal();
        modal.classList.remove('modal-fade-out');
    } else {
        console.error('El modal no es válido o no tiene el método showModal.');
    }
}

function kardexHandlers() {
    // Verifica si estamos en la vista de entrada
    const cantidadEntry = document.getElementById('Cantidad');
    if (cantidadEntry) {
        // Cambiar el evento 'change' por 'input'
        cantidadEntry.addEventListener('input', function () {
            const stockAnterior = parseInt(document.getElementById('StockAnterior').value) || 0;
            const cantidad = parseInt(this.value) || 0;
            document.getElementById('StockActual').value = stockAnterior + cantidad;
        });
    }

    // Verifica si estamos en la vista de salida
    const cantidadExit = document.getElementById('CantidadExit');
    if (cantidadExit) {
        cantidadExit.addEventListener('input', function () {
            const stockAnterior = parseInt(document.getElementById('StockAnterior').value) || 0;
            const cantidad = parseInt(this.value) || 0;
            if (cantidad > stockAnterior) {
                alert("La cantidad a retirar no puede ser mayor al stock actual.");
                this.value = stockAnterior;
                // Se recalcula el stock actual
                document.getElementById('StockActual').value = stockAnterior - parseInt(this.value) || 0;
                return;
            }
            document.getElementById('StockActual').value = stockAnterior - cantidad;
        });
    }
}

function handleCuponesForm(create = true) {
    const fechaInicio = document.querySelector('#TF_FechaInicio');
    const fechaFin = document.querySelector('#TF_FechaFin');
    const tipoDescuento = document.querySelector('#TC_TipoDescuento');
    const symbolColon = document.getElementById('symbolColon');
    const symbolPorc = document.getElementById('symbolPorc');

    // Verificar que todos los elementos necesarios existen
    if (!fechaInicio || !fechaFin || !tipoDescuento || !symbolColon || !symbolPorc) {
        console.log('No se encontraron todos los elementos necesarios para el formulario de cupones');
        return;
    }

    const today = new Date();
    const tomorrow = new Date(today);

    if (create) {
        // Obtener fecha de hoy
        fechaInicio.value = today.toISOString().split('T')[0];
        // Obtener fecha de mañana
        tomorrow.setDate(today.getDate() + 1);
        fechaFin.value = tomorrow.toISOString().split('T')[0];
    }

    // Actualizar fecha mínima de fin cuando cambie la fecha de inicio
    fechaInicio.addEventListener('change', function () {
        if (fechaInicio.value) {
            const selectedDate = new Date(fechaInicio.value);
            const minDate = new Date(selectedDate);
            minDate.setDate(selectedDate.getDate() + 1);

            const year = minDate.getFullYear();
            const month = String(minDate.getMonth() + 1).padStart(2, '0');
            const day = String(minDate.getDate()).padStart(2, '0');
            fechaFin.min = `${year}-${month}-${day}`;
        }
    });

    // Manejar la visualización de símbolos según el tipo de descuento
    function updateSymbols() {
        if (tipoDescuento.value === "P") {
            symbolColon.style.display = 'none';
            symbolPorc.style.display = 'inline';
        } else if (tipoDescuento.value === "M") {
            symbolColon.style.display = 'inline';
            symbolPorc.style.display = 'none';
        }
    }

    // Actualizar símbolos inicialmente
    updateSymbols();

    // Agregar listener para cambios
    tipoDescuento.addEventListener('change', updateSymbols);
}
