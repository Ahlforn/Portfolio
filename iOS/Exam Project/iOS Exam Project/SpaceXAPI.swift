//
//  SpaceXAPI.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 04/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit

enum PatchType: String {
    case small, large
}

struct SpaceXAPICodable: Codable {
    
}

class SpaceXAPI {
    
    static let launchesUrl = URL(string: "https://api.spacexdata.com/v3/launches")!
    static let launchPadUrl = URL(string: "https://api.spacexdata.com/v3/launchpads/")!
    
    static func fetchJSON(_ url: URL, callback: @escaping (JSONDecoder, Data) -> ()) {
        let httpTask = URLSession.shared.dataTask(with: url) {
            (data, res, err) in
            guard let data = data, err == nil else {
                debugPrint(err.debugDescription)
                return
            }
            
            let decoder = JSONDecoder()
            decoder.dateDecodingStrategy = .secondsSince1970
            callback(decoder, data)
        }
        
        httpTask.resume()
    }
    
    static func fetchLaunches(callback: @escaping ([Launch]) -> ()) {
        var launches: [Launch] = []
        
        fetchJSON(launchesUrl) {
            (decoder, data) in
            do {
                let decoded = try decoder.decode([LaunchData].self, from: data)
                
                for result in decoded {
                    launches.append(Launch(data: result, patch_small: nil, patch_large: nil, launchPad: nil))
                }
                DispatchQueue.main.async {
                    callback(launches)
                }
            }
            catch {
                debugPrint(error)
            }
        }
    }
    
    static func fetchLaunchPadInfo(_ id: String, callback: @escaping (LaunchPad) -> ()) {
        let url = URL(string: "\(id)", relativeTo: launchPadUrl)!
        debugPrint(url.absoluteString)
        fetchJSON(url) {
            (decoder, data) in
            do {
                let launchPad = try decoder.decode(LaunchPad.self, from: data)
                
                DispatchQueue.main.async {
                    callback(launchPad)
                }
            }
            catch {
                debugPrint(error)
            }
        }
    }
    
    static func fetchMissionPatch(url: URL, callback: @escaping (UIImage) -> ()) {
        
        DispatchQueue.global(qos: .userInteractive).async {
            do {
                let data = try Data(contentsOf: url)
                if let image = UIImage(data: data) {
                    callback(image)
                }
            }
            catch {
                debugPrint(error)
            }
        }
    }
}
