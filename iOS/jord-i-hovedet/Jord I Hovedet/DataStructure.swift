import Foundation
import Firebase

struct User {
    let id: String
    let soiltracerId: String
    let username: String
}

struct Measurement {
    var id: String?
    let fieldId: String
    let userId: String
    
    let latitude: Double
    let longitude: Double
    let airTemperature: Float
    let soilTemperature: Float
    let ph: Float
    let humidity: Float
    let moisture: Float
    let ec: Float
    
    let measurementComment: String?
    
    init(dictionary: [String: Any]) {
        let geoPoint = dictionary["location"] as? GeoPoint ?? GeoPoint.init(latitude: -1, longitude: -1)
        
        id = dictionary["id"] as? String
        fieldId = (dictionary["field"] as? DocumentReference ?? Firestore.firestore().collection("field").document("ndKvJdGvyBLmuMAMCd3H")).documentID
        userId = (dictionary["user"] as? DocumentReference as? DocumentReference ?? Firestore.firestore().collection("users").document("ekR3Sfhy7LQnLg8Qo7uXpD5XOxS2")).documentID
        latitude = geoPoint.latitude
        longitude = geoPoint.longitude
        airTemperature = dictionary["airTemperature"] as? Float ?? -1.0
        soilTemperature = dictionary["soilTemperature"] as? Float ?? -1.0
        ph = dictionary["ph"] as? Float ?? -1.0
        humidity = dictionary["humidity"] as? Float ?? -1.0
        moisture = dictionary["moisture"] as? Float ?? -1.0
        ec = dictionary["ec"] as? Float ?? -1.0
        measurementComment = dictionary["comment"] as? String ?? ""
    }
    
    init(fieldId: String, userId: String, latitude: Double, longitude: Double, airTemperature: Float, soilTemperature: Float, ph: Float, humidity: Float, moisture: Float, ec: Float) {
        self.fieldId = fieldId
        self.userId = userId
        self.latitude = latitude
        self.longitude = longitude
        self.airTemperature = airTemperature
        self.soilTemperature = soilTemperature
        self.ph = ph
        self.humidity = humidity
        self.moisture = moisture
        self.ec = ec
        self.measurementComment = ""
    }
}

struct Field {
    let id: String?
    let userId: String
    let name: String
}
