const mongoose = require('mongoose');
const Schema = mongoose.Schema;

const jokeSchema = new Schema({
    setup: String,
    punchline: String
});

module.exports = mongoose.model('Joke', jokeSchema);