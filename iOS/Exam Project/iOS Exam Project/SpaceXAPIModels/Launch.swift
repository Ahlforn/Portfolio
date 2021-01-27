//
//  Launch.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 04/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit

struct Launch {
    let data: LaunchData
    var patch_small: UIImage?
    var patch_large: UIImage?
    var launchPad: LaunchPad?
}

struct LaunchData: Codable {
    let flight_number: Int?
    let mission_name: String?
    let upcoming: Bool?
    let launch_year: String?
    let launch_date_unix: Date?
    let is_tentative: Bool?
    let tentative_max_precision: String?
    let tbd: Bool?
    let rocket: Rocket?
    let launch_site: LaunchSite?
    let launch_success: Bool?
    let launch_failure_details: LaunchFailureDetails?
    let links: LaunchLinks?
}

struct Rocket: Codable {
    let rocket_id: String?
    let rocket_name: String?
    let rocket_type: String?
    let first_stage: RocketFirstStage?
    let second_stage: RocketSecondStage?
    let fairings: RocketFairings?
}

struct RocketFirstStage: Codable {
    let cores: [RocketCore]?
}

struct RocketCore: Codable {
    let core_serial: String?
    let flight: Int?
    let block: Int?
    let gridfins: Bool?
    let legs: Bool?
    let reused: Bool?
    let land_success: Bool?
    let landing_intent: Bool?
    //let landing_type: Any
    //let landing_vehicle: Any
}

struct RocketSecondStage: Codable {
    let block: Int?
    let payloads: [RocketPayload]?
}

struct RocketPayload: Codable {
    let payload_id: String?
    let reused: Bool?
    let customers: [String]?
    let nationality: String?
    let manufacturer: String?
    let payload_type: String?
    let payload_mass_kg: Float?
    let payload_mass_lbs: Float?
    let orbit: String?
}

struct RocketFairings: Codable {
    let reused: Bool?
    let recovery_attempt: Bool?
    let recovered: Bool?
}

struct LaunchSite: Codable {
    let site_id: String?
    let site_name: String?
    let site_name_long: String?
}

struct LaunchFailureDetails: Codable {
    let time: Int?
    let altitude: Float?
    let reason: String?
}

struct LaunchLinks: Codable {
    let mission_patch: String?
    let mission_patch_small: String?
    let reddit_campaign: String?
    let reddit_launch: String?
    let reddit_recovery: String?
    let reddit_media: String?
    let presskit: String?
    let article_link: String?
    let wikipedia: String?
    let video_link: String?
    let youtube_id: String?
    let flickr_images: [String]?
}
