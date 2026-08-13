const serviceSearch = document.getElementById("serviceSearch");
const serviceResults = document.getElementById("serviceResults");

serviceSearch.addEventListener("input", function () //Run this code - user types,deletes or changes the text.
{
    let term = this.value; //serviceSearch input
    if (term.length < 2)
    {
        serviceResults.innerHTML = "";
        return;
    }
    fetch("/Services/Search?term=" + encodeURIComponent(term))
        .then(response => response.json())
        .then(data => {
            serviceResults.innerHTML = ""; //remove old suggestions
              data.forEach(service =>
               {
                 let div = document.createElement("div");
                 div.className = "border p-2";
                  div.innerHTML = service.name + " - ₹" + service.price;
                  //When the user clicks a service, call a function named addService
                 div.onclick = function ()
                {
                  addService(service.id,service.name,service.price);
                  serviceSearch.value = "";
                  serviceResults.innerHTML = "";
                };

                serviceResults.appendChild(div);
            });
        });
});