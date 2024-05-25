function loadList(nameFilter) {
    
    axios.get('https://localhost:7149/api/PizzaWeb/GetAllPizzas',
        {
            params:
            {
                name: nameFilter
            }
        }).then((res) => {
            if (res.data.length == 0) 
            {
                console.log("res.data");
                document.querySelector('.js_no_pizza').classList.remove('d-none');
                document.querySelector('.js_pizza_table').classList.add('d-none');
            }
            else {
                document.querySelector('.js_no_pizza').classList.add('d-none');
                var postTable = document.querySelector('.js_pizza_table');
                postTable.classList.remove('d-none');

                postTable.innerHTML = '';
                res.data.forEach((post) => {
                    console.log(post);
                    postTable.innerHTML += `
                            <div class="col">
                                <img src="${pizza}" />
                                <div class="card">
                                    <div class="card-body">
                                        <h5 class="card-title">${pizza}</h5>
                                        <p class="card-text">${pizza}</p>
                                    </div>
                                    <div class="btn btn-danger" onclick="deleteById(${pizza})">Elimina</div>
                                </div>
                            </div>`;
                });
            }
        });
}

function deleteById(id) {
    axios.delete("/api/PostWeb/DeletePost/" + id)
        .then((res) => {
            loadList('');
        });
}

loadList('');

function search() {

    console.log(this.value);
    loadList(this.value);
}

document.querySelector('.js_search').addEventListener('keyup', search)