const controller = require('../../controllers/controller');

module.exports = function(express) {
    const router = express.Router();

    router.route('/on')
        .get((req, res) => {
            controller.submitServiceToRegistry().then((response) => {
                if(response.ok)
                    res.status(200).json({ result: 'Successfully submitted to registry.' });
                else
                    res.status(500).json({ result: 'Operation failed.', data: response });
            }).catch((err) => {
                console.log(err);
            })
        });

    router.route('/off')
        .get((req, res) => {
            controller.unsubmitServiceFromRegistry().then((response) => {
                if(response.ok)
                    res.status(200).json({ result: 'Successfully unsubmitted to registry.' });
                else
                    res.status(500).json({ result: 'Operation failed.', data: response });
            }).catch((err) => {
                console.log(err);
            })
        });

    return router;
}