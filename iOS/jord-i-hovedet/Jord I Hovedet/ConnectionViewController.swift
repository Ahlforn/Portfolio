//
//  ConnectionViewController.swift
//  Jord I Hovedet
//
//  Created by dmu mac 02 on 29/04/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import UIKit
import Foundation
import CoreBluetooth

class ConnectionViewController: UIViewController, CBCentralManagerDelegate {//UITableViewController {
    var centralManager: CBCentralManager!
    var peripheral: CBPeripheral?
    var characteristics: CBCharacteristic!
    var peripherals: [CBPeripheral]!
    var data: String = ""
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        centralManager = CBCentralManager(delegate: self, queue: nil)
        peripherals = []
    }
    
    private func showAlert(message: String) {
        let alert = UIAlertController(title: "Measurement", message: message, preferredStyle: .alert)
        
        alert.addAction(UIAlertAction(title: "Okay", style: .default, handler: nil))
        
        present(alert, animated: true)
    }
    
    func startScan() {
        switch centralManager.state {
        case .poweredOn:
            centralManager.scanForPeripherals(withServices: nil, options: nil)
            
            stopScanning(after: 5.0)
        default:
            break
        }
    }
    
    func centralManagerDidUpdateState(_ central: CBCentralManager) {
        startScan()
    }
    
    func centralManager(_ central: CBCentralManager, didDiscover peripheral: CBPeripheral, advertisementData: [String : Any], rssi RSSI: NSNumber) {
        peripherals.append(peripheral)
        if let name = peripheral.name, name == "BLETest" {
            self.peripheral = peripheral
            
            centralManager.connect(peripheral, options: ["CBConnectPeripheralOptionNotifyOnNotificationKey": true])
        }
    }
    
    func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
        self.peripheral = peripheral
        self.peripheral?.delegate = self
        
        centralManager.stopScan()
        
        self.peripheral?.discoverServices(nil)
    }
    
    func centralManager(_ central: CBCentralManager, didFailToConnect peripheral: CBPeripheral, error: Error?) {
        showAlert(message: "Couldn't connect to the peripheral")
    }
    
    func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        showAlert(message: "Disconnected from the peripheral")
    }
    
    func stopConnection() {
        if (centralManager.isScanning) {
            centralManager.stopScan()
        }
        
        if let peripheral = peripheral {
            centralManager.cancelPeripheralConnection(peripheral)
        }
    }
    
    private func stopScanning(after seconds: Double) {
        weak var timer: Timer?
        
        timer?.invalidate()
        
        timer = Timer.scheduledTimer(withTimeInterval: seconds, repeats: true) { [unowned self] _ in
            if self.centralManager.isScanning {
                self.centralManager.stopScan()
                
                timer?.invalidate()
                
                self.showAlert(message: "Unable to find peripheral")
            }
        }
    }
    
    @IBAction func onConnectClick(_ sender: Any) {
        startScan()
    }
}

extension ConnectionViewController: CBPeripheralDelegate {
    func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        guard let services = peripheral.services else {return}
        
        for service in services {
            self.peripheral?.discoverCharacteristics(nil, for: service)
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didDiscoverCharacteristicsFor service: CBService, error: Error?) {
        guard let characteristics = service.characteristics else {return}
        
        for characteristic in characteristics {
            if characteristic.uuid.uuidString == "FFE1" {
                self.peripheral?.readValue(for: characteristic)
                self.peripheral?.setNotifyValue(true, for: characteristic)
                self.characteristics = characteristic
            }
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic, error: Error?) {
        let dataChunk = String(bytes: characteristic.value!, encoding: .utf8) ?? ""
        data += dataChunk
        
        if data.last == "*" {
            data = String(data.suffix(data.count - 1)) //Sletter 'start of header'-karakteren i starten af datasættet
            let fields = data.split(separator: ",")
            data = ""
            if fields.count > 0 {
                let latitude = Double(fields[0])!
                let longitude = Double(fields[1])!
                let airTemperature = Float(fields[2])!
                let soilTemperature = Float(fields[3])!
                let ph = Float(fields[4])!
                let humidity = Float(fields[5])!
                let moisture = Float(fields[6])!
                let ec = Float(fields[7])!
                
                let fbService = FirebaseService.getInstance()
                let measurement = Measurement(fieldId: "ndKvJdGvyBLmuMAMCd3H", userId: "W4o9z2eI41fpjtf6KP7nHjwd3bd2", latitude: latitude, longitude: longitude, airTemperature: airTemperature, soilTemperature: soilTemperature, ph: ph, humidity: humidity, moisture: moisture, ec: ec)
                
                fbService.takeMeasurement(measurement: measurement) {
                    self.showAlert(message: String(describing: measurement))
                }
            }
        }
    }
}
