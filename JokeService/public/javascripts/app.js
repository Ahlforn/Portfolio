let dropdown = document.getElementById('serviceSelect');
let services = document.getElementsByClassName('service');

dropdown.addEventListener('change', (event) => {
    for(let i = 0; i < services.length; i++) {
        services[i].classList.remove('shown');

        if(services[i].getAttribute('id') === dropdown.value) {
            services[i].classList.add('shown');
        }
    }
});

dropdown.selectedIndex = 0;
document.querySelector('div#_').classList.add('shown');
