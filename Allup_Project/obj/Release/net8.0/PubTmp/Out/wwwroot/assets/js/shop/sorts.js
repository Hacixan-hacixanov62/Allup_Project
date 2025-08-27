$(function () {
    const container = document.getElementById("productContainer"); // məhsul kartlarının olduğu div
    const loadMoreBtn = document.getElementById("loadMoreBtn"); // varsa 'Load More' düyməsi

    let skip = 0;
    let isSortedOrFiltered = false;

    // Sənin range sliderinin JS hissəsi
    $("#slider-range").slider({
        range: true,
        min: 0,
        max: 1000,
        step: 10,
        values: [@Model.SelectedMinPrice, @Model.SelectedMaxPrice],
        slide: function (event, ui) {
            $("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);

            // Hidden inputlara dəyərləri yaz
            $("#minPriceInput").val(ui.values[0]);
            $("#maxPriceInput").val(ui.values[1]);
        },
        change: function (event, ui) {
            const minPrice = ui.values[0];
            const maxPrice = ui.values[1];

            // UI reset
            skip = 0;
            isSortedOrFiltered = true;
            container.innerHTML = "";
            loadMoreBtn.style.display = "none";

            fetch(`/Shop/FilterByPrice?min=${minPrice}&max=${maxPrice}`)
                .then(res => res.json())
                .then(data => {
                    if (data.length === 0) {
                        container.innerHTML = "<p>No products found in this range.</p>";
                        return;
                    }

                    data.forEach(item => {
                        const html = renderProduct(item);
                        container.insertAdjacentHTML("beforeend", html);
                    });
                })
                .catch(err => console.error("Price filter error:", err));
        }
    });

    // İlk açılışda göstər
    $("#amount").val("$" + $("#slider-range").slider("values", 0) +
        " - $" + $("#slider-range").slider("values", 1));
});

// Məhsulun HTML kartını qaytaran funksiya
function renderProduct(product) {
    return `
    <div class="col-12 col-sm-6 col-lg-4 mb-4">
        <div class="product-card">
            <img src="${product.image}" alt="${product.name}">
                <h5>${product.name}</h5>
                <p>$${product.price}</p>
        </div>
    </div>`;
}