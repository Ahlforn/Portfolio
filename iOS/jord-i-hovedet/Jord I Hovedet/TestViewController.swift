//
//  TestViewController.swift
//  Jord I Hovedet
//
//  Created by Lasse Borgen on 29/04/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import UIKit

class TestViewController: UIViewController {

    var firebaseService: FirebaseService?
    
    override func viewDidLoad() {
        super.viewDidLoad()
        firebaseService = FirebaseService.getInstance()
        
        testLogin(email: "g@gmail.com", password: "pass123")
        //testSignUp(email: "fds@gmail.com", password: "jordihovedet")
        //testTakeMeasurement()
        //testGetMeasurements()
        testAddCommentToMeasurement()
    }
    
    @IBAction func onTestNibClicked(_ sender: Any) {
        if let nib = Bundle.main.loadNibNamed(NewField.nibName, owner: self, options: nil) {
            let newFieldView = nib.first as! NewField
            newFieldView.owner = self
            newFieldView.show()
        }
    }
    func testAddCommentToMeasurement() {
        let addCommentCompletionHandler: () -> () = {
            print("Added comment to measurement!")
        }
        var measurement = Measurement(fieldId: "ndKvJdGvyBLmuMAMCd3H", userId: "W4o9z2eI41fpjtf6KP7nHjwd3bd2", latitude: 10.0, longitude: 10.0, airTemperature: 37.0, soilTemperature: 37.0, ph: 7.0, humidity: 10.0, moisture: 10.0, ec: 10.0)
        measurement.id = "SXruTvb4sGQI6pGLCvjo"
        
        firebaseService!.addCommentToMeasurement(measurement: measurement, comment: "Send Nudes")
        
        
        testGetMeasurements()
    }
    
    func testGetMeasurements() {
        let getMeasurementsHandler: ([Measurement]) -> () = {measurements in
            print("FOUND MEASUREMENTS!!")
            for measurement in measurements {
                print(measurement)
            }
        }
        
        firebaseService!.getMeasurementsFor(field: Field(id: "ndKvJdGvyBLmuMAMCd3H", userId: "W4o9z2eI41fpjtf6KP7nHjwd3bd2", name: "Mark1"), completionHandler: getMeasurementsHandler)
    
    }
    
    func testTakeMeasurement() {
        let measurement = Measurement(fieldId: "ndKvJdGvyBLmuMAMCd3H", userId: "W4o9z2eI41fpjtf6KP7nHjwd3bd2", latitude: 10.0, longitude: 10.0, airTemperature: 37.0, soilTemperature: 37.0, ph: 7.0, humidity: 10.0, moisture: 10.0, ec: 10.0)
        firebaseService!.takeMeasurement(measurement: measurement) {
            print("MEASUREMENT TAKEN SUCCESFULLY!")
        }
    }
    
    func testSignUp(email: String, password: String) {
        let signUpHandler: () -> () = {
            print("SIGN-UP SUCCESS!")
            self.testLogin(email: email, password: password)
        }
        
        let errorHandler: () -> () = {
            print("SIGN-UP FAIL")
        }
        firebaseService!.signUp(email: email, password: password, completionHandler: signUpHandler, errorHandler: errorHandler)
    }
    
    func testLogin(email: String, password: String) {
        let loginHandler: () -> () = {
            print("LOGGED IN!!!")
            self.testAddCommentToMeasurement()
        }
        let errorHandler: () -> () = {
            print("LOGIN-FAIL!!!")
        }
        
        firebaseService!.login(email: email, password: password, completionHandler: loginHandler, errorHandler: errorHandler)
    }

}
