const controller = require('../controllers/controller');
const fs = require('fs').promises;
const config = require('../config');
const { body, query } = require('express-validator/check');

module.exports = function(express) {
    let router = express.Router();

    router.route('/')
        .get((req, res, next) => {
            controller.getJokes()
                .then((jokes) => {
                    let ourService = {
                        _id: '_',
                        name: config.appTitle,
                        jokes: jokes
                    };

                    fs.readFile(config.serviceCache)
                        .then((data) => {
                            let json = JSON.parse(data);
                            json.unshift(ourService);

                            res.render('index', { title: config.appTitle, services: json });
                        });
                });
        });

    router.route('/add')
        .get((req, res, next) => {
            res.render('form');
        });

    router.route('/edit/:id')
        .get(
            [
                query('id').not().isEmpty().trim().escape()
            ],
            (req, res, next) => {
                controller.getJoke(req.params.id)
                    .then((joke) => {
                        res.render('form', { joke: joke[0] });
                    });
        });

    return router;
};