document.addEventListener('DOMContentLoaded', () => {
        // Завантаження артистів через API
        fetch('/api/shopapi/artists')
            .then(res => res.json())
            .then(artists => {
                const artistList = document.getElementById('artistList');
                artistList.innerHTML = '';

                if (artists.length === 0) {
                    artistList.innerHTML = '<li>Артисти відсутні</li>';
                } else {
                    artists.sort((a, b) => a.name.localeCompare(b.name));
                    artists.forEach(artist => {
                        const li = document.createElement('li');
                        const a = document.createElement('a');
                        a.href = `/Shop/Artists/Artist/${artist.id}`;
                        a.textContent = artist.name;
                        li.appendChild(a);
                        artistList.appendChild(li);
                    });
                }
            })
            .catch(err => {
                console.error('Помилка завантаження артистів:', err);
                const artistList = document.getElementById('artistList');
                artistList.innerHTML = '<li>Помилка завантаження</li>';
            });

        // Оновлення ціни при зміні кількості
        function updateAlbumPrice(priceBlock) {
            const qty = parseInt(priceBlock.querySelector('.qty-input').value);
            const basePrice = parseFloat(priceBlock.querySelector('.add-to-cart-btn').dataset.price);
            const priceDisplay = priceBlock.querySelector('.price-highlight');
            priceDisplay.textContent = `${(qty * basePrice).toFixed(2)} грн.`;
        }

        document.querySelectorAll('.btn-qty.plus').forEach((btn) => {
            btn.addEventListener('click', () => {
                const priceBlock = btn.closest('.price-block');
                const qtyInput = priceBlock.querySelector('.qty-input');
                qtyInput.value = parseInt(qtyInput.value) + 1;
                updateAlbumPrice(priceBlock);
            });
        });

        document.querySelectorAll('.btn-qty.minus').forEach((btn) => {
            btn.addEventListener('click', () => {
                const priceBlock = btn.closest('.price-block');
                const qtyInput = priceBlock.querySelector('.qty-input');
                if (parseInt(qtyInput.value) > 1) {
                    qtyInput.value = parseInt(qtyInput.value) - 1;
                    updateAlbumPrice(priceBlock);
                }
            });
        });

        // Додавання товару в кошик
        document.querySelectorAll('.add-to-cart-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const priceBlock = btn.closest('.price-block');
                const id = parseInt(btn.dataset.albumId);
                const artist = btn.dataset.artist;
                const title = btn.dataset.title;
                const image = btn.dataset.image || '';
                const price = parseFloat(btn.dataset.price);
                const qty = parseInt(priceBlock.querySelector('.qty-input').value);

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
        });

        updateCartUI();
    });

    // Пошук
    document.addEventListener("DOMContentLoaded", () => {
        const input = document.getElementById("searchInput");
        const dropdown = document.getElementById("searchDropdown");
        let timer;

        input.addEventListener("input", () => {
            clearTimeout(timer);
            const query = input.value.trim();

            if (query.length === 0) {
                dropdown.style.display = "none";
                dropdown.innerHTML = "";
                return;
            }

            timer = setTimeout(() => {
                fetch(`?handler=Search&query=${encodeURIComponent(query)}`)
                    .then(res => res.json())
                    .then(data => {
                        if (data.length === 0) {
                            dropdown.style.display = "block";
                            dropdown.innerHTML = "Нічого не знайдено";
                            return;
                        }

                        dropdown.innerHTML = data.map(item => `
                            <a href="/Shop/Albums/Details/${item.id}" class="search-item">
                                <img src="${item.image || '/img/placeholder.jpg'}" alt="${item.title}" />
                                <div class="search-text">
                                    <strong>${item.title}</strong>
                                    <span>${item.artist}</span>
                                </div>
                            </a>
                        `).join("");

                        dropdown.style.display = "block";
                    });
            }, 300);
        });

        document.addEventListener("click", e => {
            if (!e.target.closest("#searchForm")) {
                dropdown.style.display = "none";
            }
        });
    });
