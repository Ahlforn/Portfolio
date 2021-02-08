//
//  LoginViewController.swift
//  MakeableChat
//
//  Created by Anders Hofmeister on 29/01/2021.
//

import UIKit

class LoginViewController: UIViewController {
    @IBOutlet weak var textfield: UITextField!
    override func viewDidLoad() {
        super.viewDidLoad()
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "JoinChat" {
            let vc = segue.destination as! MessageViewController
            vc.username = textfield.text ?? ""
        }
    }
}
