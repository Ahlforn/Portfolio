//
//  LaunchPad.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 05/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import Foundation

struct LaunchPad: Codable {
    let id: Int?
    let status: String?
    let location: Location?
    let vehicles_launched: [String]?
    let attempted_launches: Int?
    let successful_launches: Int?
    let wikipedia: String?
    let details: String?
    let site_is: String?
    let site_name_long: String?
}

struct Location: Codable {
    let name: String?
    let region: String?
    let latitude: Double?
    let longitude: Double?
}
