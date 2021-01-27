//
//  ViewController.swift
//  Jord I Hovedet
//
//  Created by dmu mac 03 on 24/04/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import UIKit
import Firebase

class LoginViewController: UIViewController {

    
    @IBOutlet weak var email: UITextField! {
        didSet {
            email.delegate = self
        }
    }
    @IBOutlet weak var password: UITextField! {
        didSet {
            password.delegate = self
        }
    }
    @IBOutlet weak var infoCommitter: UIButton!
    
    var state: String?
    var fbService: FirebaseService!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        fbService = FirebaseService.getInstance()
        state = "Login"
    }
    
    @IBAction func switchStateAction(_ sender: UISegmentedControl) {
        switch sender.selectedSegmentIndex {
        case 0:
            state = "Login"
            infoCommitter.setTitle("Log In", for: .normal)
        default:
            state = "Signup"
            infoCommitter.setTitle("Sign Up", for: .normal)
        }
        
    }
    
    @IBAction func commitInfoAction(_ sender: Any) {
        let emailText = email.text!
        let passwordText = password.text!
        let errorHandler: () -> () = {
            let alertController = UIAlertController(title: "Error", message: "A database error occured", preferredStyle: .alert)
            let defaultAction = UIAlertAction(title: "OK", style: .cancel, handler: nil)
            
            alertController.addAction(defaultAction)
            self.present(alertController, animated: true, completion: nil)
        }
        let loginHandler: () -> () = {
            let userDefaults = UserDefaults.standard
            userDefaults.set(emailText, forKey: "email")
            userDefaults.set(passwordText, forKey: "password")
            self.performSegue(withIdentifier: "loginToHome", sender: self)
        }
        if state == "Login" {
            fbService.login(email: emailText, password: passwordText, completionHandler: loginHandler, errorHandler: errorHandler)
        }
        else if state == "Signup" {
            let signUpHandler: () -> () = {
                self.fbService.login(email: emailText, password: passwordText, completionHandler: loginHandler, errorHandler: errorHandler)
            }
            fbService.signUp(email: emailText, password: passwordText, completionHandler: signUpHandler, errorHandler: errorHandler)
        }
    }
}

extension LoginViewController : UITextFieldDelegate {
    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        textField.resignFirstResponder()
        return true
    }
}
