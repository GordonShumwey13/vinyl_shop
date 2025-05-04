// Оновлення кошика
function updateCartUI() {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    let totalQty = 0;
    let totalPrice = 0;

    cart.forEach(item => {
        totalQty += parseInt(item.qty);
        totalPrice += item.qty * item.price;
    });

    const countEl = document.querySelector('.cart-count');
    const totalEl = document.querySelector('.cart-total');

    if (countEl && totalEl) {
        countEl.textContent = totalQty;
        totalEl.textContent = `${totalPrice.toFixed(2)} грн.`;
    }
}

document.addEventListener('DOMContentLoaded', updateCartUI);

// Відкриття модального вікна авторизації
function openAuthModal() {
    const modal = document.getElementById("authModal");
    if (modal) {
        modal.style.display = "block";
    }
}

// Закриття модального вікна авторизації
function closeAuthModal() {
    const modal = document.getElementById("authModal");
    if (modal) {
        modal.style.display = "none";
    }
}

// Додавання обробника події для кнопки "Увійти"
window.addEventListener("message", function (event) {
    if (event.data?.action === "loginSuccess") {
        closeAuthModal();
        location.reload();
    }

    if (event.data?.action === "closeModal") {
        closeAuthModal();
    }
});
