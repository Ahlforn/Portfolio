//
//  ViewController.swift
//  iOS Exam Project
//
//  Created by Anders Hofmeister on 03/06/2019.
//  Copyright © 2019 Anders Hofmeister Brønden. All rights reserved.
//

import UIKit
import FirebaseDatabase

class LaunchesViewController: UIViewController {
    
    var ref: DatabaseReference?
    var markRef: DatabaseReference?
    var markHandleAdd: DatabaseHandle?
    var markHandleRemove: DatabaseHandle?
    var commentRef: DatabaseReference?
    var commentHandleAdd: DatabaseHandle?
    var commentHandleRemove: DatabaseHandle?
    var commentHandleChange: DatabaseHandle?
    weak var stateController: StateController?
    @IBOutlet weak var launchesTableView: UITableView! {
        didSet {
            launchesTableView.dataSource = self
            launchesTableView.delegate = self
        }
    }
    
    func updateMark(_ snapshot: DataSnapshot, state: Bool) {
        if let mark = snapshot as DataSnapshot?, let nr = Int(mark.key), let index = self.stateController?.getLaunchIndexFromFlightNumber(nr) {
            stateController?.updateMarks(state, for: index)
            
            launchesTableView.reloadData()
        }
    }
    
    func updateComment(_ snapshot: DataSnapshot, remove: Bool = false) {
        if let nr = Int(snapshot.key), let index = stateController?.getLaunchIndexFromFlightNumber(nr) {
            if remove {
                stateController?.updateComment(nil, for: index)
            } else {
                stateController?.updateComment(snapshot.value as? String, for: index)
            }
            
            launchesTableView.reloadData()
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        markRef = Database.database().reference().child("marks")
        markHandleAdd = markRef?.observe(.childChanged) {
            (snapshot) in
            self.updateMark(snapshot, state: true)
        }
        markHandleRemove = markRef?.observe(.childRemoved) {
            (snapshot) in
            self.updateMark(snapshot, state: false)
        }
        
        commentRef = Database.database().reference().child("comments")
        commentHandleAdd = commentRef?.observe(.childAdded) {
            (snapshot) in
            self.updateComment(snapshot)
        }
        commentHandleRemove = commentRef?.observe(.childRemoved) {
            (snapshot) in
            self.updateComment(snapshot, remove: true)
        }
        commentHandleChange = commentRef?.observe(.childChanged) {
            (snapshot) in
            self.updateComment(snapshot)
        }
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        if let databaseHandle = markHandleAdd {
            markRef?.removeObserver(withHandle: databaseHandle)
        }
        if let databaseHandle = markHandleRemove {
            markRef?.removeObserver(withHandle: databaseHandle)
        }
        if let databaseHandle = commentHandleAdd {
            markRef?.removeObserver(withHandle: databaseHandle)
        }
        if let databaseHandle = commentHandleRemove {
            markRef?.removeObserver(withHandle: databaseHandle)
        }
        if let databaseHandle = commentHandleChange {
            markRef?.removeObserver(withHandle: databaseHandle)
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "details" {
            let vc = segue.destination as? DetailsViewController
            vc?.stateController = stateController
            
            if let indexPath = launchesTableView.indexPathForSelectedRow {
                vc?.launchIndex = indexPath.row
                launchesTableView.deselectRow(at: indexPath, animated: false)
            }
        }
    }
    
    func markLaunch(_ index: Int) {
        if let ref = markRef, let key = stateController?.launches[index].data.flight_number, let marked = stateController?.marks.contains(index) {
            let marks = stateController?.marks
            let val = marked ? nil : ""
            ref.child("\(key)").setValue(val)
        }
    }
    
}

extension LaunchesViewController: StateControllerDelegate {
    func gotData() {
        launchesTableView.reloadData()
    }
}

extension LaunchesViewController: UITableViewDataSource {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return stateController?.launches.count ?? 0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let launch = stateController?.launches[indexPath.row]
        let cell = launchesTableView.dequeueReusableCell(withIdentifier: "LaunchCell", for: indexPath) as! LaunchTableViewCell
        
        cell.name?.text = launch?.data.mission_name
        
        if let date = launch?.data.launch_date_unix {
            let dateFormatter = DateFormatter()
            dateFormatter.dateFormat = Constants.dateFormat
            
            cell.date?.text = dateFormatter.string(from: date)
        }
        
        if let result = launch?.data.launch_success {
            cell.result?.text = result ? "Succeeded" : "Failed"
        }
        
        cell.missionPatch?.clipsToBounds = true
        if launch?.patch_small != nil {
            cell.missionPatch?.image = launch?.patch_small
        } else {
            cell.missionPatch?.image = nil
            
            if let url = launch?.data.links?.mission_patch_small, let imageUrl = URL(string: url) {
                SpaceXAPI.fetchMissionPatch(url: imageUrl) {
                    [weak self]
                    (image) in
                    self?.stateController?.updatePatch(image, for: indexPath.row, type: .small)
                    DispatchQueue.main.async {
                        self?.launchesTableView.reloadRows(at: [indexPath], with: .fade)
                    }
                }
            }
        }
        
        if let marked = stateController?.marks.contains(indexPath.row) {
            cell.markIcon.isHidden = !marked
        }
        
        return cell
    }
}

extension LaunchesViewController: UITableViewDelegate {
    func tableView(_ tableView: UITableView, leadingSwipeActionsConfigurationForRowAt indexPath: IndexPath) -> UISwipeActionsConfiguration? {
        let mark = UIContextualAction(style: .normal, title: "Mark") {
            [weak self]
            (action, view, nil) in
            self?.markLaunch(indexPath.row)
            self?.launchesTableView.reloadData()
        }
        mark.backgroundColor = Constants.launchMarkButtonColor
        
        let comment = UIContextualAction(style: .normal, title: "Comment") {
            [weak self]
            (action, view, nil) in
            
        }
        comment.backgroundColor = Constants.launchCommentButtonColor
        
        return UISwipeActionsConfiguration(actions: [mark, comment])
    }
    
    func tableView(_ tableView: UITableView, trailingSwipeActionsConfigurationForRowAt indexPath: IndexPath) -> UISwipeActionsConfiguration? {
        return UISwipeActionsConfiguration(actions: [])
    }
}

