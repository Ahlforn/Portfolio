//
//  BluetoothService.swift
//  Jord I Hovedet
//
//  Created by dmu mac 02 on 24/04/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import Foundation
import CoreBluetooth

class BluetoothService: CBCentralManagerDelegate {
    var peripheralName: String!
    var centralManager: CBCentralManager!
    var peripheral: CBPeripheral?
    var characteristics: CBCharacteristic!
    
    init(peripheralName: String) {
        super.init()
        
        self.peripheralName = peripheralName
        self.centralManager = CBCentralManager(delegate: self, queue: nil)
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
        if let name = peripheral.name, name == peripheralName {
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
        // TODO: Couldn't connect
    }
    
    func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        // TODO: Connection lost
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
                
                // TODO: Unable to find device
            }
        }
    }
}

extension BluetoothService: CBPeripheralDelegate {
    func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        guard let services = peripheral.services else {return}
        
        for service in services {
            self.peripheral?.discoverCharacteristics(nil, for: service)
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didDiscoverCharacteristicsFor service: CBService, error: Error?) {
        guard let characteristics = service.characteristics else {return}
        
        for characteristic in characteristics {
            self.peripheral?.readValue(for: characteristic)
            self.peripheral?.setNotifyValue(true, for: characteristic)
            self.characteristics = characteristic
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic, error: Error?) {
        let data = String(bytes: characteristic.value!, encoding: .utf8)
        
        if let datas = data?.split(separator: ",") {
            let latitude = Double(datas[0])!
            let longitude = Double(datas[1])!
            let airTemperature = Float(datas[2])!
            let soilTemperature = Float(datas[3])!
            let ph = Float(datas[4])!
            let humidity = Float(datas[5])!
            let moisture = Float(datas[6])!
            let ec = Float(datas[7])!
            
            let measurement = Measurement(fieldId: "tmp", userId: "tmp", latitude: latitude, longitude: longitude, airTemperature: airTemperature, soilTemperature: soilTemperature, ph: ph, humidity: humidity, moisture: moisture, ec: ec)
        }
    }
}
