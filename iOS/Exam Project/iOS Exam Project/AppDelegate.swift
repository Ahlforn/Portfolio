//
//  AppDelegate.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 03/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit
import Firebase

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {

    var window: UIWindow?
    let stateController = StateController()

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        
        FirebaseApp.configure()
        
        if let navigationVC = window?.rootViewController as? UINavigationController {
            let launchesVC = navigationVC.viewControllers.first as? LaunchesViewController
            stateController.delegate = launchesVC
            launchesVC?.stateController = stateController
        }
        
        return true
    }


}

