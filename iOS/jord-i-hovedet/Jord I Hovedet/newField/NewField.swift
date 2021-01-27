import UIKit

class NewField: UIView {
    static let nibName = "NewField"
    var owner: UIViewController?
    
    @IBOutlet weak var createFieldView: UIView! {
        didSet {
            createFieldView.layer.borderWidth = 2
            createFieldView.layer.borderColor = UIColor.black.cgColor
        }
    }
    @IBOutlet weak var textField: UITextField! {
        didSet {
            textField.delegate = self
        }
    }
    
    @IBAction func createButtonClicked() {
        let fbService = FirebaseService.getInstance()
        fbService.createField(field: Field(id: nil, userId: fbService.user!.uid, name: textField.text!)) {
            (self.owner as? FieldListController)?.fetchFields(newFieldAdded: true)
        }
        self.dismiss()
    }
    
    @IBAction func dismiss() {
        UIView.animate(withDuration: 0.5, animations: {
            self.alpha = 0.0
        }) { (done) in
            self.owner!.navigationItem.rightBarButtonItem?.isEnabled = true
            self.removeFromSuperview()
        }
    }
    
    func show() {
        self.alpha = 0.0
        self.frame = owner!.view.frame
        owner!.view.addSubview(self)
        
        UIView.animate(withDuration: 0.5, animations: {
            self.alpha = 1.0
        })
        owner!.navigationItem.rightBarButtonItem?.isEnabled = false
    }
}

extension NewField: UITextFieldDelegate {
    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        if textField.text?.count == 0 {
            return false
        } else {
            createButtonClicked()
            dismiss()
            return true
        }
    }
}
