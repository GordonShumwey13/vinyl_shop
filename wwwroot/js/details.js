document.addEventListener('DOMContentLoaded', () => {
    const minusBtn = document.querySelector('.btn-qty.minus');
    const plusBtn = document.querySelector('.btn-qty.plus');
    const qtyInput = document.querySelector('.qty-input');
    const pricePerOne = parseFloat(document.getElementById('pricePerOne').textContent);
    const totalPriceElement = document.getElementById('totalPrice');

    // Встановлення початкової ціни
    function updateTotalPrice(qty) {
        const totalPrice = (pricePerOne * qty).toFixed(2);
        totalPriceElement.textContent = totalPrice;
    }

    minusBtn.addEventListener('click', () => {
        let val = parseInt(qtyInput.value);
        if (val > 1) {
            qtyInput.value = --val;
            updateTotalPrice(val);
        }
    });

    plusBtn.addEventListener('click', () => {
        let val = parseInt(qtyInput.value);
        qtyInput.value = ++val;
        updateTotalPrice(val);
    });

    // Оновлення ціни при зміні кількості
    const addToCartBtn = document.getElementById('addToCart');
    if (addToCartBtn) {
        addToCartBtn.addEventListener('click', () => {
            const id = parseInt(addToCartBtn.dataset.albumId);
            const artist = addToCartBtn.dataset.artist;
            const title = addToCartBtn.dataset.title;
            const price = parseFloat(addToCartBtn.dataset.price);
            const image = addToCartBtn.dataset.image;
            const qty = parseInt(qtyInput.value);

            let cart = JSON.parse(localStorage.getItem('cart')) || [];
            let item = cart.find(i => i.id === id);

            if (item) {
                item.qty += qty;
            } else {
                cart.push({ id, artist, title, qty, price, image });
            }

            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartUI();
        });
    }

    // Оновлення інтерфейсу кошика
    const ratingStars = document.querySelectorAll('.album-rating i');
    ratingStars.forEach(star => {
        star.addEventListener('click', () => {
            const ratingContainer = document.querySelector('.album-rating');
            const albumId = parseInt(ratingContainer.dataset.albumId);

            fetch('/Shop/Albums/Rate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    albumId: albumId,
                    rating: rating
                })
            })
                .then(response => {
                    if (response.ok) {
                        location.reload();
                    } else {
                        alert('Помилка при оцінюванні.');
                    }
                })
                .catch(error => {
                    console.error('Помилка:', error);
                });
        });
    });
});
