//
//  StateController.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 04/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit
import FirebaseDatabase

protocol StateControllerDelegate {
    func gotData()
}

struct Comment {
    var index: Int
    var text: String?
}

class StateController {
    var delegate: StateControllerDelegate?
    
    private(set) var launches = [Launch]()
    private(set) var marks = [Int]()
    private(set) var comments = [Comment]()
    
    init() {
        SpaceXAPI.fetchLaunches() {
            [weak self]
            (launches) in
            self?.launches = launches
            DispatchQueue.main.async {
                self?.delegate?.gotData()
            }
        }
        
        
    }
    
    // Often flight number and index will match, but we can't be sure
    func getLaunchIndexFromFlightNumber(_ nr: Int) -> Int? {
        if let found = launches.map({ $0.data.flight_number }).firstIndex(of: nr) {
            return found
        }
        
        return nil
    }
    
    func updatePatch(_ image: UIImage, for index: Int, type: PatchType) {
        switch type {
        case .large:
            launches[index].patch_large = image
        case .small:
            launches[index].patch_small = image
        }
    }
    
    func updateLaunchSiteInfo(_ launchPad: LaunchPad, for index: Int) {
        launches[index].launchPad = launchPad
    }
    
    func updateMarks(_ state: Bool, for index: Int) {
        if state, !marks.contains(index) {
            marks.removeFirst(index)
        }
        else if !state, marks.contains(index) {
            marks.append(index)
        }
    }
    
    // index: index of the launch
    // if text is nil, we will search for the index of the comment related to the launch
    func updateComment(_ text: String?, for index: Int) {
        if text == nil, let found = comments.map({ $0.index }).firstIndex(of: index) {
            comments.remove(at: found)
        } else {
            let comment = Comment(index: index, text: text)
            comments.append(comment)
        }
    }
}
