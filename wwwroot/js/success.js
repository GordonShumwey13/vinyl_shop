
    document.addEventListener('DOMContentLoaded', function () {
        // Очистити кеш
        localStorage.removeItem("cart");

        // Оновити значення у хедері
        const cartCount = document.querySelector('.cart-count');
        const cartTotal = document.querySelector('.cart-total');

        if (cartCount) cartCount.textContent = '0';
        if (cartTotal) cartTotal.textContent = '0.00 грн.';

        console.log("🧼 Кошик очищено та інтерфейс оновлено.");
    });
