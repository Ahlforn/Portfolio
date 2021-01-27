//
//  DetailsViewController.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 05/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit

class DetailsViewController: UIViewController {

    weak var stateController: StateController?
    var launchIndex: Int?
    
    @IBOutlet weak var missionPatch: UIImageView!
    @IBOutlet weak var missionName: UILabel!
    @IBOutlet weak var launchDate: UILabel!
    @IBOutlet weak var launchSuccess: UILabel!
    @IBOutlet weak var launchImages: UIButton!
    @IBOutlet weak var launchSite: UIButton!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        if let index = launchIndex {
            let launch = stateController?.launches[index]
            
            missionName.text = launch?.data.mission_name
            
            if let date = launch?.data.launch_date_unix {
                let dateFormatter = DateFormatter()
                dateFormatter.dateFormat = Constants.dateFormat
                
                launchDate.text = dateFormatter.string(from: date)
            }
            
            if let success = launch?.data.launch_success {
                launchSuccess.text = success ? "Succeeded" : "Failed"
            }
            
            if launch?.patch_large != nil {
                missionPatch.image = launch?.patch_large
            } else {
                missionPatch.image = nil
                
                if let url = launch?.data.links?.mission_patch, let imageUrl = URL(string: url) {
                    SpaceXAPI.fetchMissionPatch(url: imageUrl) {
                        [weak self]
                        (image) in
                        DispatchQueue.main.async {
                            self?.missionPatch.image = image
                        }
                    }
                }
            }
            
            launchImages.isHidden = launch?.data.links?.flickr_images == nil
            
            if launch?.launchPad != nil {
                launchSite.isEnabled = true
            } else {
                launchSite.isEnabled = false
                
                if let id = launch?.data.launch_site?.site_id {
                    SpaceXAPI.fetchLaunchPadInfo(id) {
                        [weak self]
                        (launchPad) in
                        self?.stateController?.updateLaunchSiteInfo(launchPad, for: index)
                        self?.launchSite.isEnabled = true
                    }
                }
            }
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        switch segue.identifier {
        case "launchSite":
            let vc = segue.destination as? LaunchSiteViewController
            vc?.stateController = stateController
            vc?.launchIndex = launchIndex
        default:
            break
        }
    }
}
