const searchPizzaInputDOM = document.getElementById("pizzaSearchInput");
searchPizzaInputDOM.addEventListener("keyup", searchPizzas);

loadPizzas();
searchPizzas();

// FUNCTIONS
function loadPizzas() {
	const pizzasLoaderDOM = document.querySelector("#our-pizzas-data .loader");
	const pizzasListDOM = document.querySelector("#our-pizzas-data .data-list");
	const pizzasEmptyDOM = document.querySelector("#our-pizzas-data .empty-data-list");

	axios.get('/api/Pizzas/GetAll')
		.then(response => {
			if (response.data.length == 0) {
				pizzasLoaderDOM.classList.add("d-none");
				pizzasEmptyDOM.classList.remove("d-none");
			} else {
				for (i = 0; i < 4; i++) {
					let pizza = response.data[i];
					pizzasListDOM.innerHTML += generatePizzaDOM(pizza.name, pizza.description, pizza.price, pizza.imgPath);
				}
				pizzasListDOM.classList.remove("d-none");
				pizzasLoaderDOM.classList.add("d-none");
			}
		});
}
function searchPizzas() {
	const searchLoaderDOM = document.querySelector("#search-pizzas-data .loader");
	const searchListDOM = document.querySelector("#search-pizzas-data .data-list");
	const searchEmptyDOM = document.querySelector("#search-pizzas-data .empty-data-list");

	let name = searchPizzaInputDOM.value;
	let emptyInputUrl = '/api/Pizzas/GetAll';

	searchLoaderDOM.classList.remove("d-none");
	axios.get(name ? '/api/Pizzas/GetAllContaining' : emptyInputUrl,
		{
			params: {name}
		}).then(response => {
			searchListDOM.innerHTML = "";
			searchLoaderDOM.classList.add("d-none");

			if (response.data.length == 0) {
				searchEmptyDOM.classList.remove("d-none");
				searchListDOM.classList.add("d-none");
			} else {
				searchEmptyDOM.classList.add("d-none");
				response.data.forEach(pizza => {
					searchListDOM.innerHTML += generatePizzaDOM(pizza.name, pizza.description, pizza.price, pizza.imgPath);
				});
				searchListDOM.classList.remove("d-none");
			}
		})
}
function generatePizzaDOM(title, description, price, imgPath) {
    return`
    <div class="col">
		<div class="card h-100 shadow">
			<img src="${imgPath}" class="card-img-top pizza-img" alt="">
			<div class="card-body">
				<h5 class="card-title">${title}</h5>
				<p class="card-text">${description}</p>
			</div>
			<div class="card-footer">
				<small class="text-body-secondary">&euro; ${price}</small>
			</div>
		</div>
	</div>
    `
}