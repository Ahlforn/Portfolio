//
//  MapViewController.swift
//  Jord I Hovedet
//
//  Created by dmu mac 03 on 29/04/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import UIKit
import MapKit
import CoreLocation

class FieldAnnotation: NSObject, MKAnnotation {
    var coordinate: CLLocationCoordinate2D
    var index: Int
    var title: String? = "[PH] Title"
    var subtitle: String? = "[PH] Subtitle"
    
    init(coordinate: CLLocationCoordinate2D, index: Int) {
        self.coordinate = coordinate
        self.index = index
    }
}

class MapViewController: UIViewController, MKMapViewDelegate {
    var fbService: FirebaseService? = nil
    var measurements: [Measurement] = []
    let locationManager = CLLocationManager()
    var field: Field?
    
    @IBOutlet weak var locationMapView: MKMapView! {
        didSet {
            locationMapView.delegate = self
            locationMapView.mapType = .satellite
        }
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        fbService = FirebaseService.getInstance()
        let measurementHandler: ([Measurement]) -> () = { (newMeasurements) in
            if newMeasurements.count > 0 {
                var lat = newMeasurements[0].latitude
                var long = newMeasurements[0].longitude
                
                let coordinate = CLLocationCoordinate2D(latitude: CLLocationDegrees(lat), longitude: CLLocationDegrees(long))
                let region = MKCoordinateRegion(center: coordinate, latitudinalMeters: 500, longitudinalMeters: 500)
                self.locationMapView.setRegion(region, animated: true)
                
                for (index, measurement) in newMeasurements.enumerated() {
                    self.measurements.append(measurement)
                    lat = measurement.latitude
                    long = measurement.longitude
                    let coordinate = CLLocationCoordinate2D(latitude: CLLocationDegrees(lat), longitude: CLLocationDegrees(long))
                    let fieldAnnotation = FieldAnnotation(coordinate: coordinate, index: index)
                    self.locationMapView.addAnnotation(fieldAnnotation)
                }
            }
        }
        fbService?.getMeasurementsFor(field: field!, completionHandler: measurementHandler)
    }
    
    func mapView(_ mapView: MKMapView, viewFor annotation: MKAnnotation) -> MKAnnotationView? {
        if !(annotation is FieldAnnotation) {
            print("Returner nil")
            return nil
        }
        
        let reuseId = "pin"
        
        var pinView = mapView.dequeueReusableAnnotationView(withIdentifier: reuseId)
        if pinView == nil {
            pinView = MKPinAnnotationView(annotation: annotation, reuseIdentifier: reuseId)
            pinView?.canShowCallout = true
            
            let rightButton = UIButton(type: .detailDisclosure)
            rightButton.title(for: UIControl.State.normal)
            
            pinView!.rightCalloutAccessoryView = rightButton as UIView
        }
        else {
            pinView?.annotation = annotation
        }
        
        return pinView
    }
    
    func mapView(_ mapView: MKMapView, annotationView view: MKAnnotationView, calloutAccessoryControlTapped control: UIControl) {
        if control == view.rightCalloutAccessoryView {
            performSegue(withIdentifier: "showPointDetailSegue", sender: view)
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        print("Gør klar til segue")
        print("Sender: \(sender!)")
        if (segue.identifier == "showPointDetailSegue" ) {
            let vc = segue.destination as! MeasureController
            
            let annotation = (sender as? MKAnnotationView)!.annotation
            let index = (annotation as! FieldAnnotation).index
            vc.data = measurements[index]
        }
        
    }
}
