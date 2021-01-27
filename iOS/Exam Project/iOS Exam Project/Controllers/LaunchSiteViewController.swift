//
//  LaunchSiteViewController.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 05/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit
import MapKit

class LaunchSiteViewController: UIViewController {

    weak var stateController: StateController?
    var launchIndex: Int?
    let regionRadius = 1000.0
    @IBOutlet weak var launchSiteMap: MKMapView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        launchSiteMap.delegate = self

        if let index = launchIndex, let launch = stateController?.launches[index], let lat = launch.launchPad?.location?.latitude, let long = launch.launchPad?.location?.longitude {
            let coordinate = CLLocationCoordinate2D(latitude: lat, longitude: long)
            let region = MKCoordinateRegion(center: coordinate, latitudinalMeters: regionRadius, longitudinalMeters: regionRadius)
            launchSiteMap.mapType = .satellite
            launchSiteMap.setRegion(region, animated: true)
            
            if let title = launch.launchPad?.location?.name, let subtitle = launch.launchPad?.details {
                let siteAnnotation = LaunchSiteAnnotation(coordinate: coordinate)
                siteAnnotation.title = title
                siteAnnotation.subtitle = subtitle
                launchSiteMap.addAnnotation(siteAnnotation)
            }
        }
    }
}

extension LaunchSiteViewController: MKMapViewDelegate {
    func mapView(_ mapView: MKMapView, viewFor annotation: MKAnnotation) -> MKAnnotationView?
    {
        let annotationView = MKAnnotationView(annotation: annotation, reuseIdentifier: "launchSite")
        annotationView.image =  UIImage(named: "rocket")
        annotationView.canShowCallout = true
        
        if let subtitle = annotation.subtitle {
            let detailLabel = UILabel()
            detailLabel.numberOfLines = 0
            detailLabel.font = detailLabel.font.withSize(12)
            detailLabel.text = subtitle
            
            annotationView.detailCalloutAccessoryView = detailLabel
        }
        
        return annotationView
    }
}

class LaunchSiteAnnotation: NSObject, MKAnnotation {
    var coordinate: CLLocationCoordinate2D
    var title: String?
    var subtitle: String?
    
    init(coordinate: CLLocationCoordinate2D) {
        self.coordinate = coordinate
    }
}
