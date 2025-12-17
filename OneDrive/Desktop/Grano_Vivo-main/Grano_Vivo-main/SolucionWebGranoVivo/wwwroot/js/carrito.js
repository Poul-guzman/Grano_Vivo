function mostrarAlerta(mensaje, tipo = 'success') {
    const alertContainer = document.getElementById('alert-container');
    const alertId = 'alert-' + Date.now();
    const alertClass = tipo === 'success' ? 'alert-success' : 'alert-danger';
    const icon = tipo === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const alertHtml = `
        <div id="${alertId}" class="alert ${alertClass} alert-dismissible fade show" role="alert" style="min-width: 300px;">
            <i class="fas ${icon} me-2"></i>${mensaje}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    alertContainer.insertAdjacentHTML('beforeend', alertHtml);
    
    setTimeout(() => {
        const alert = document.getElementById(alertId);
        if (alert) {
            alert.remove();
        }
    }, 5000);
}

function actualizarContador(cantidad) {
    const contador = document.getElementById('contador-carrito');
    const contadorFlotante = document.getElementById('contador-carrito-flotante');
    const contadorModal = document.getElementById('contador-carrito-modal');
    
    if (contador) {
        contador.textContent = cantidad;
    }
    if (contadorFlotante) {
        contadorFlotante.textContent = cantidad;
    }
    if (contadorModal) {
        contadorModal.textContent = cantidad;
    }
}

function actualizarTotales(data) {
    if (data.subtotal) {
        const subtotalEl = document.getElementById('subtotal-carrito');
        if (subtotalEl) subtotalEl.textContent = 'S/ ' + data.subtotal;
    }
    if (data.impuesto) {
        const impuestoEl = document.getElementById('impuesto-carrito');
        if (impuestoEl) impuestoEl.textContent = 'S/ ' + data.impuesto;
    }
    if (data.total) {
        const totalEl = document.getElementById('total-carrito');
        if (totalEl) totalEl.textContent = 'S/ ' + data.total;
    }
}

document.addEventListener('DOMContentLoaded', function() {
   
    document.querySelectorAll('.agregar-carrito').forEach(button => {
        button.addEventListener('click', async function() {
            const productoId = parseInt(this.getAttribute('data-producto-id'));
            const productoNombre = this.getAttribute('data-producto-nombre');
            const productoPrecio = parseFloat(this.getAttribute('data-producto-precio'));
            const productoImagen = this.getAttribute('data-producto-imagen');
            const productoStock = parseInt(this.getAttribute('data-producto-stock'));
            
            if (this.disabled) {
                mostrarAlerta('Este producto no tiene stock disponible.', 'danger');
                return;
            }

            this.disabled = true;
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Agregando...';

            try {
                const response = await fetch('?handler=AgregarAlCarrito', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: `productoId=${productoId}&productoNombre=${encodeURIComponent(productoNombre)}&productoPrecio=${productoPrecio}&productoImagen=${encodeURIComponent(productoImagen)}&productoStock=${productoStock}&cantidad=1`
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const text = await response.text();
                let data;
                
                try {
                    data = JSON.parse(text);
                } catch (parseError) {
                    console.error('Error parsing JSON:', parseError);
                    console.error('Response text:', text);
                    throw new Error('Respuesta inválida del servidor');
                }

                if (data.success) {
                    mostrarAlerta(`${productoNombre} agregado al carrito.`, 'success');
                    actualizarContador(data.cantidad);
              
                    setTimeout(() => {
                        window.location.reload();
                    }, 500);
                } else {
                    mostrarAlerta(data.message || 'Error al agregar el producto.', 'danger');
                    this.disabled = false;
                    this.innerHTML = 'AÑADIR AL CARRITO';
                }
            } catch (error) {
                console.error('Error completo:', error);
                console.error('Stack:', error.stack);
                mostrarAlerta('Error al agregar el producto al carrito: ' + error.message, 'danger');
                this.disabled = false;
                this.innerHTML = 'AÑADIR AL CARRITO';
            }
        });
    });

    document.querySelectorAll('.cantidad-input').forEach(input => {
        input.addEventListener('change', async function() {
            const productoId = parseInt(this.getAttribute('data-producto-id'));
            const cantidad = parseInt(this.value);

            if (cantidad < 1) {
                this.value = 1;
                mostrarAlerta('La cantidad debe ser al menos 1.', 'danger');
                return;
            }

            try {
                const response = await fetch('?handler=ActualizarCantidad', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: `productoId=${productoId}&cantidad=${cantidad}`
                });

                const data = await response.json();

                if (data.success) {
                    const itemContainer = this.closest('.carrito-item');
                    const subtotalEl = itemContainer.querySelector('.item-subtotal');
                    if (subtotalEl) {
                        subtotalEl.textContent = data.subtotal;
                    }

                    actualizarTotales({
                        subtotal: data.totalCarrito,
                        impuesto: data.impuesto,
                        total: data.total
                    });
                } else {
                    mostrarAlerta(data.message || 'Error al actualizar la cantidad.', 'danger');
                 
                    window.location.reload();
                }
            } catch (error) {
                console.error('Error:', error);
                mostrarAlerta('Error al actualizar la cantidad.', 'danger');
                window.location.reload();
            }
        });
    });

    document.querySelectorAll('.eliminar-item').forEach(button => {
        button.addEventListener('click', async function() {
            const productoId = parseInt(this.getAttribute('data-producto-id'));
            const itemContainer = this.closest('.carrito-item');
            const productoNombre = itemContainer.querySelector('h6')?.textContent || 'Producto';

            if (!confirm(`¿Está seguro de eliminar ${productoNombre} del carrito?`)) {
                return;
            }

            try {
                const response = await fetch('?handler=EliminarDelCarrito', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: `productoId=${productoId}`
                });

                const data = await response.json();

                if (data.success) {
                    mostrarAlerta('Producto eliminado del carrito.', 'success');
                    actualizarContador(data.cantidad);
                    
                    itemContainer.remove();

                    actualizarTotales({
                        subtotal: data.totalCarrito,
                        impuesto: data.impuesto,
                        total: data.total
                    });

               
                    if (data.cantidad === 0) {
                        setTimeout(() => {
                            window.location.reload();
                        }, 1000);
                    } else {
                        setTimeout(() => {
                            window.location.reload();
                        }, 500);
                    }
                } else {
                    mostrarAlerta(data.message || 'Error al eliminar el producto.', 'danger');
                }
            } catch (error) {
                console.error('Error:', error);
                mostrarAlerta('Error al eliminar el producto del carrito.', 'danger');
            }
        });
    });

    const formConfirmarPedido = document.getElementById('formConfirmarPedido');
    if (formConfirmarPedido) {
        formConfirmarPedido.addEventListener('submit', async function(e) {
            e.preventDefault();

            const nombreCliente = document.getElementById('nombreCliente').value;
            const emailCliente = document.getElementById('emailCliente').value;
            const telefonoCliente = document.getElementById('telefonoCliente').value;
            const direccionCliente = document.getElementById('direccionCliente').value;

            const submitButton = this.querySelector('button[type="submit"]');
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Procesando...';

            try {
                const response = await fetch('?handler=ConfirmarPedido', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: `nombreCliente=${encodeURIComponent(nombreCliente)}&emailCliente=${encodeURIComponent(emailCliente)}&telefonoCliente=${encodeURIComponent(telefonoCliente)}&direccionCliente=${encodeURIComponent(direccionCliente)}`
                });

                const data = await response.json();

                if (data.success) {
                    mostrarAlerta(data.message, 'success');
                    
                    const modal = bootstrap.Modal.getInstance(document.getElementById('modalConfirmarPedido'));
                    if (modal) {
                        modal.hide();
                    }

                    setTimeout(() => {
                        window.location.reload();
                    }, 2000);
                } else {
                    mostrarAlerta(data.message || 'Error al confirmar el pedido.', 'danger');
                    submitButton.disabled = false;
                    submitButton.innerHTML = 'Confirmar Compra';
                }
            } catch (error) {
                console.error('Error:', error);
                mostrarAlerta('Error al confirmar el pedido.', 'danger');
                submitButton.disabled = false;
                submitButton.innerHTML = 'Confirmar Compra';
            }
        });
    }
});

