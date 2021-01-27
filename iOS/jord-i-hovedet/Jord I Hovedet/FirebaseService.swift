import Foundation
import FirebaseAuth
import Firebase

class FirebaseService {
    let db: Firestore
    
    //Singleton-halløj.... Det er altså nemmere end at passe den samme FirebaseService-instans fra den ene view-controller til den anden gennem segues
    private static var instance: FirebaseService = {
        let firebaseService = FirebaseService()
        return firebaseService
    }()
    private init() {
        db = Firestore.firestore()
    }
    class func getInstance() -> FirebaseService {
        return instance
    }
    
    var user: FirebaseAuth.User?
    var fields: [Field]?
    
    func login(email: String, password: String, completionHandler: @escaping () -> Void, errorHandler: @escaping () -> Void) {
        Auth.auth().signIn(withEmail: email, password: password) { user, error in
            if let error = error {
                print(error)
                errorHandler()
            } else {
                self.user = user?.user
                completionHandler()
            }
        }
    }
    
    func signUp(email: String, password: String, completionHandler: @escaping () -> Void, errorHandler: @escaping () -> Void) {
        Auth.auth().createUser(withEmail: email, password: password) { (result, error) in
            if let error = error {
                print(error)
                errorHandler()
            } else {
                completionHandler()
            }
        }
    }
    
    func takeMeasurement(measurement: Measurement, completionHandler: @escaping () -> Void) {
        let fieldReference = db.collection("fields").document(measurement.fieldId)
        let userReference = db.collection("users").document(measurement.userId)
        let location = GeoPoint.init(latitude: measurement.latitude, longitude: measurement.longitude)
        
        db.collection("measurements").addDocument(data: ["field": fieldReference, "user": userReference, "location": location, "airTemperature": measurement.airTemperature, "soilTemperature": measurement.soilTemperature,"ph": measurement.ph, "humidity": measurement.humidity, "moisture": measurement.moisture, "ec": measurement.ec]) { error in
                if let error = error {
                    print(error)
                } else {
                    completionHandler()
                }
        }
    }
    
    func getMeasurementsFor(field: Field, completionHandler: @escaping ([Measurement]) -> Void) {
        var result = [Measurement]()
        
        if let fieldId = field.id {
            let fieldReference = db.collection("fields").document(fieldId)
            
            db.collection("measurements").whereField("field", isEqualTo: fieldReference).getDocuments { (snapshot, error) in
                if let documents = snapshot?.documents {
                    for document in documents {
                        var documentData = document.data()
                        documentData["id"] = document.documentID
                        let measurement = Measurement(dictionary: documentData)
                        result.append(measurement)
                    }
                }
                completionHandler(result)
            }
        }
    }
    
    func addCommentToMeasurement(measurement: Measurement, comment: String) {
        guard let measurementId = measurement.id else { return }
        db.collection("measurements").document(measurementId).updateData(["comment" : comment]) { error in
            if let error = error {
                print(error)
            }
        }
    }
    
    func getFieldsFor(user: FirebaseAuth.User, completionHandler: @escaping ([Field]) -> Void) {
        var result = [Field]()
        
        let userReference = db.collection("users").document(user.uid)
        db.collection("fields").whereField("user", isEqualTo: userReference).getDocuments { (snapshot, error) in
            if let documents = snapshot?.documents {
                for document in documents {
                    let field = Field(id: document.documentID, userId: user.uid, name: document.data()["name"] as? String ?? "Ugyldigt mark-navn")
                    result.append(field)
                }
            }
            completionHandler(result)
            self.fields = result
        }
    }
    
    func createField(field: Field, completionHandler: @escaping () -> Void) {
        let userReference = db.collection("users").document(field.userId)
        
        db.collection("fields").addDocument(data: ["name": field.name, "user": userReference]) { error in
            if let error = error {
                print(error)
            } else {
                completionHandler()
            }
        }
    }
}
