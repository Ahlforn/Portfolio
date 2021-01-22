const controller = require('../../controllers/controller');
const { body, query } = require('express-validator/check');

module.exports = function(express) {
    const router = express.Router();

    router.route('/services')
        .get((req, res) => {

            controller.getJokesFromServices()
                .then((services) => {
                    res.json(services);
                })
                .catch((err) => {
                    res.status(501).send('Error: operation failed.');
                });
        });

    router.route('/')
        .get((req, res) => {
            controller.getJokes()
                .then((jokes) => {
                    res.json(jokes);
                })
                .catch((err) => {
                    throw err
                }
            );
        })
        .post(
            [
                body('setup').not().isEmpty().trim().escape(),
                body('punchline').not().isEmpty().trim().escape()
            ],
            (req, res) => {
                controller.createJoke(req.body.setup, req.body.punchline)
                    .then(() => {
                        res.status(201).send('Joke created.');
                    })
                    .catch((err) => {
                        res.status(501).send('Error: operation failed.');
                    });
        })
        .put(
            [
                body('id').not().isEmpty().trim().escape(),
                body('setup').not().isEmpty().trim().escape(),
                body('punchline').not().isEmpty().trim().escape()
            ],
            (req, res) => {
                controller.updateJoke(req.body.id, req.body.setup, req.body.punchline)
                    .then(() => {
                        res.status(200).send('Joke updated.');
                    })
                    .catch((err) => {
                        res.status(501).send('Error: operation failed.');
                        throw err;
                    });
        });

    router.route('/:id')
        .get(
            [
                query('id').not().isEmpty().trim().escape()
            ],
            (req, res) => {
                controller.getJoke(req.params.id)
                    .then((joke) => {
                        res.json(joke);
                    })
                    .catch((err) => {
                        throw err;
                    });
        })
        .delete(
            [
                query('id').not().isEmpty().trim().escape()
            ],
            (req, res) => {
                controller.deleteJoke(req.params.id)
                    .then(() => {
                        res.status(200).send('Joke deleted.');
                    })
                    .catch((err) => {
                        res.status(501).send('Error: operation failed.');
                    });
        });

    return router;
};
