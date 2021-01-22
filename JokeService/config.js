let config = {
    appTitle: 'Comedy Comrade',
    appUrl: 'https://dip-ls-jokeservice.herokuapp.com',
    mongoDBhost: 'mongodb://diplsjokeservice:yEwM9NTCti9mPdff3JgYaNrhjtKVzeEf@ds155663.mlab.com:55663/jokeservice',
    mongooseConnectOptions: {
        useNewUrlParser: true,
        useCreateIndex: true,
        autoIndex: true
    },
    registry: {
        url: 'https://krdo-joke-registry.herokuapp.com',
        secret: '01110011011100110110100001101000' // binary for ascii "sshh"
    },
    serviceCache: './serviceCache.json',
    bannedServices: [
        'https://dip-ls-jokeservice.herokuapp.com',
        'https://jokeservicedpo.herokuapp.com',
        'http://ph-klitt-nixa-jokeservice.herokuapp.com',
        'http://funnyjokeservice.herokuapp.com',
        'https://joke-service-3000.herokuapp.com',
        'http://jokeservice-jeppe-steen.herokuapp.com',
        'https://joke-service-poi.herokuapp.com',
        'https://testerbois.herokuapp.com'
    ]

};

module.exports = config;