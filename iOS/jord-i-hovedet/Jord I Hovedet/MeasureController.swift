//
//  MeasureController.swift
//  Jord I Hovedet
//
//  Created by dmu mac 05 on 01/05/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import Foundation
import UIKit

class MeasureController: UIViewController {
    
    var data: Measurement?
    var fbService: FirebaseService!
    var commentTemp: String = ""
    
    @IBOutlet weak var airTemperatureTF: UITextField!
    @IBOutlet weak var humidityTF: UITextField!
    @IBOutlet weak var soilTemperatureTF: UITextField!
    @IBOutlet weak var moistureTF: UITextField!
    @IBOutlet weak var phTF: UITextField!
    @IBOutlet weak var ecTF: UITextField!
    @IBOutlet weak var commentTV: UITextView!
    @IBOutlet weak var updateCommentBTN: UIButton!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        fbService = FirebaseService.getInstance()
        airTemperatureTF.text = data?.airTemperature.description
        humidityTF.text = data?.humidity.description
        soilTemperatureTF.text = data?.soilTemperature.description
        moistureTF.text = data?.moisture.description
        phTF.text = data?.ph.description
        ecTF.text = data?.ec.description
        if(data?.measurementComment == nil){
            commentTV.text = commentTemp
        } else {
            commentTV.text = data?.measurementComment?.description
        }
        updateCommentBTN.layer.cornerRadius = 0.5
        commentTV.layer.cornerRadius = 0.5
    }
    
    @IBAction func updateComment(_ sender: UIButton) {
        commentTemp = commentTV.text
        fbService.addCommentToMeasurement(measurement: data!, comment: commentTemp)
        print("Tilføjet kommentar til measure")
    }
}

