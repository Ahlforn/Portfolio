const Joke = require('../models/joke');
const fetch = require('node-fetch');
const config = require('../config');

exports.createJoke = (setup, punchline) => {
    if(!setup || !punchline) throw 'Setup or punchline missing.';

    const joke = new Joke({
        setup: setup,
        punchline: punchline
    });

    return joke.save();
};

exports.getJokes = () => {
    return Joke.find().exec();
};

exports.getJoke = (id) => {
    return Joke.find({ _id: id }).exec();
};

exports.updateJoke = (id, setup, punchline) => {
    return Joke.findOneAndUpdate({_id: id}, {setup: setup, punchline: punchline}).exec();
};

exports.deleteJoke = (id) => {
    return Joke.findOneAndDelete({_id: id}).exec();
};


exports.registerService = (name, url, secret) => {
    const body = {
        name: name,
        address: url,
        secret: secret
    };

    return fetch(config.registry.url + 'api/services', {
        method: 'post',
        body: JSON.stringify(body),
        headers: { 'Content-Type': 'application/json' }
    });
};

exports.unregisterService = (url, secret) => {
    const body = {
        address: url,
        secret: secret
    };

    return fetch(config.registry.url + 'api/services', {
        method: 'delete',
        body: JSON.stringify(body),
        headers: { 'Content-Type': 'application/json' }
    });
};

exports.getServices = () => {
    return new Promise((resolve, reject) => {
        fetch(config.registry.url + '/api/services', { timeout: 5000 })
            .then(res => {
                if(res.ok) {
                    res.json()
                        .then((json) => {
                            resolve(json);
                        })
                } else {
                    reject('Unsuccessful request to registry.');
                }
            })
            .catch(err => {
                reject(err);
            });
    });
};

exports.getJokesFromService = (url) => {
    return new Promise((resolve, reject) => {
        if(!url) reject('Empty url');
        fetch(url + '/api/jokes', { timeout: 5000 })
            .then(res => {
                if (res.ok) {
                    res.json()
                        .then((json) => {
                            resolve(json);
                        })
                        .catch((err) => { reject('Bad json: ' + err) });
                } else {
                    reject('Unsuccessful request to joke service: ' + url + '/api/jokes');
                }

            })
            .catch(err => {
                reject(err);
            });
    });
};

exports.getJokesFromServices = () => {
    return new Promise((resolve, reject) => {
        let finalServices = [];
        this.getServices()
            .then((services) => {
                let queriedServices = [];
                let jokePromises = [];

                for(let i = 0; i < services.length; i++) {
                    if (services[i].address && config.bannedServices.indexOf(trimSlash(services[i].address).toLowerCase()) === -1) {
                        queriedServices.push(i);
                        jokePromises.push(this.getJokesFromService(trimSlash(services[i].address)));
                    }
                }

                let promiseCount = jokePromises.length;
                for(let i = 0; i < jokePromises.length; i++) {
                    jokePromises[i].then((jokes) => {
                        if(jokes.length > 0) {
                            services[queriedServices[i]].jokes = jokes;
                            finalServices.push(services[queriedServices[i]]);
                        }

                        promiseCount--;
                        if(promiseCount === 0) resolve(finalServices);
                    }).catch((err) => {
                        promiseCount--;
                        if(promiseCount === 0) resolve(finalServices);
                    });
                }
            })
            .catch((err) => {
                reject(err);
            });
    });
};

exports.submitServiceToRegistry = () => {
    const data = {
        "name": config.appTitle,
        "address": config.appUrl,
        "secret": config.registry.secret
    };

    return fetch(config.registry.url + '/api/services', {
        method: 'POST',
        timeout: 5000,
        headers: { 'Content-Type': 'application/json; charset=utf-8' },
        body: JSON.stringify(data)
    });
};

exports.unsubmitServiceFromRegistry = () => {
    const data = {
        "address": config.appUrl,
        "secret": config.registry.secret
    };

    return fetch(config.registry.url + '/api/services', {
        method: 'DELETE',
        timeout: 5000,
        headers: { 'Content-Type': 'application/json; charset=utf-8' },
        body: JSON.stringify(data)
    });
};

function trimSlash(url) {
    return (url.substr(url.length - 1) === '/') ? url.substr(0, url.length - 1).trim() : url.trim();
}
