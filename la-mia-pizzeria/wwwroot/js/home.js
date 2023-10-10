//SETUP
const searchPizzaInputDOM = document.getElementById("pizzaSearchInput");
searchPizzaInputDOM.addEventListener("keyup", searchPizzas);
loadPizzas();
searchPizzas();

//CRUD SETUP
const createPizzaBtnDOM = document.getElementById("create-pizza-btn");
createPizzaBtnDOM.addEventListener("click", createPizza)

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
				pizzasListDOM.innerHTML = "";
				for (i = 0; i < 4; i++) {
					let pizza = response.data[i];
					pizzasListDOM.innerHTML += generatePizzaDOM(pizza);
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
			params: { name }
		}).then(response => {
			searchListDOM.innerHTML = "";
			searchLoaderDOM.classList.add("d-none");

			if (response.data.length == 0) {
				searchEmptyDOM.classList.remove("d-none");
				searchListDOM.classList.add("d-none");
			} else {
				searchEmptyDOM.classList.add("d-none");
				response.data.forEach(pizza => {
					searchListDOM.innerHTML += generatePizzaDOM(pizza);
				});
				searchListDOM.classList.remove("d-none");
			}
		});
}
function generatePizzaDOM(pizza) {
    return`
    <div class="col">
		<div class="card h-100 shadow">
			<img src="${pizza.imgPath}" class="card-img-top pizza-img" alt="">
			<div class="card-body">
				<h5 class="card-title">${pizza.name}</h5>
				<p class="card-text">${pizza.description}</p>
			</div>
			<div class="card-footer">
				<small class="text-body-secondary">&euro; ${pizza.price}</small>
			</div>
		</div>
	</div>
    `
}
function createPizza() {
	const createModal = document.getElementById("manipulate-pizza-data");
	const createLoaderDOM = createModal.querySelector(".loader");
	const createFieldsDOM = createModal.querySelector(".data-list");
	const createErrorsDOM = createModal.querySelector(".error-data");
	const createSubmitBtn = createModal.querySelector(".submit-btn")
	createErrorsDOM.classList.add("d-none");
	createModal.classList.remove("d-none");

	createSubmitBtn.addEventListener("click", () => {
		const name = createModal.querySelector("#name").value;
		const price = createModal.querySelector("#price").value;
		const description = createModal.querySelector("#description").value;
		const imgPath = createModal.querySelector("#imgPath").value;

		let pizza = {
			name,
			price,
			description,
			imgPath,
			slug : ""
		}
		axios.post("/api/Pizzas/Create", pizza)
			.then(() => {
				createModal.classList.add("d-none");
				name = "";
				price = 0;
				description = "";
				imgPath = "";
				refreshPizzas();
			}).catch(err => {
				console.log(err);
				createErrorsDOM.classList.remove("d-none");
				createErrorsDOM.innerHTML = err.message;
			});
		
	})
}
function refreshPizzas() {
	loadPizzas();
	searchPizzas();
}