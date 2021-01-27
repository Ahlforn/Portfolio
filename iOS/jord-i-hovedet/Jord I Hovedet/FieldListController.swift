//
//  FieldListController.swift
//  Jord I Hovedet
//
//  Created by Simone Louise Joergensen on 01/05/2019.
//  Copyright © 2019 dmu mac 03. All rights reserved.
//

import UIKit
import Firebase

class FieldListController: UITableViewController {
    var fields: [Field] = []
   
    @IBOutlet var tableViewFields: UITableView!
    let fbService: FirebaseService = FirebaseService.getInstance()

    @IBAction func onAddFieldButtonClicked(_ sender: UIBarButtonItem) {
        if let nib = Bundle.main.loadNibNamed(NewField.nibName, owner: self, options: nil) {
            let newFieldView = nib.first as! NewField
            newFieldView.owner = self
            newFieldView.show()
        }
    }
    override func viewDidLoad() {
        super.viewDidLoad()
        
        fetchFields(newFieldAdded: false)
    }
    
    func fetchFields(newFieldAdded: Bool) {
        let fieldHandler = { (fields: [Field]) in
            DispatchQueue.main.async {
                self.fields = fields
                self.tableViewFields.reloadData()
            }
        }
        
        if let fetchedFields = fbService.fields, fetchedFields.count > 0, !newFieldAdded {
            fieldHandler(fetchedFields)
        } else {
            fbService.getFieldsFor(user: fbService.user!, completionHandler: fieldHandler)
        }
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return fields.count
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "fieldCell", for: indexPath)
        let field = fields[indexPath.row]
        
        cell.textLabel?.text = field.name
        return cell
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if (segue.identifier == "fieldToMap") {
            let destinationVC = segue.destination as! MapViewController
            destinationVC.field = fields[tableView.indexPathForSelectedRow!.row]
        }
    }
}
