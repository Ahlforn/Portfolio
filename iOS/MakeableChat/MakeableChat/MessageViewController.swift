//
//  ViewController.swift
//  MakeableChat
//
//  Created by Anders Hofmeister on 28/01/2021.
//

import UIKit
import Firebase

struct Message {
    var username: String = ""
    var timestamp: Date = Date()
    var content: String = ""
}

class MessageViewController: UIViewController {
    @IBOutlet weak var messageList: UICollectionView!
    @IBOutlet weak var lowerConstraint: NSLayoutConstraint!
    @IBOutlet weak var textfield: UITextField!
    
    
    let db = Firestore.firestore()
    var messages: [Message] = [Message]()
    var username: String = ""

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        /*db.collection("messages").getDocuments() { (querySnapshot, err) in
            if err != nil {
                print("Error getting messages.")
            } else {
                guard let documents = querySnapshot?.documents else {
                    print("No messages")
                    return
                }
                for document in documents {
                    var message = Message()
                    message.username = document["username"] as! String
                    message.timestamp = (document["time"] as! Timestamp).dateValue()
                    message.content = document["content"] as! String
                    self.messages.append(message)
                }
                self.messageList.reloadData()
            }
        }*/
        
        db.collection("messages").order(by: "time", descending: false).addSnapshotListener {querySnapshot, err in
            if err != nil {
                print("Error getting messages.")
                return
            } else {
                guard let documents = querySnapshot?.documents else {
                    print("No messages")
                    return
                }
                
                self.messages.removeAll()
                for document in documents {
                    var message = Message()
                    message.username = document["username"] as! String
                    message.timestamp = (document["time"] as! Timestamp).dateValue()
                    message.content = document["content"] as! String
                    self.messages.append(message)
                }
                self.messageList.reloadData()
                // scroll to last item
                if self.messageList.numberOfItems(inSection: 0) > 0 {
                    let indexPath = IndexPath.init(item: self.messageList.numberOfItems(inSection: 0) - 1, section: 0)
                    self.messageList.scrollToItem(at: indexPath, at: .top, animated: false)
                }
            }
        }
        
        // Collection view layout stuff
        let size = NSCollectionLayoutSize(widthDimension: .fractionalWidth(1), heightDimension: .estimated(50))
        let item = NSCollectionLayoutItem(layoutSize: size)
        let group = NSCollectionLayoutGroup.horizontal(layoutSize: size, subitem: item, count: 1)
        let section = NSCollectionLayoutSection(group: group)
        
        let layout = UICollectionViewCompositionalLayout(section: section)
        messageList.collectionViewLayout = layout
        messageList.register(MessageCell.self, forCellWithReuseIdentifier: "MessageCell")
        messageList.register(MessageCell.self, forCellWithReuseIdentifier: "OwnMessageCell")
        
        NotificationCenter.default.addObserver(self, selector: #selector(keyboardWillShow), name: UIResponder.keyboardWillShowNotification, object: nil)
    }
    
    @objc func keyboardWillShow(_ notification: NSNotification) {
        if let keyboardRect = (notification.userInfo?[UIResponder.keyboardFrameEndUserInfoKey] as? NSValue)?.cgRectValue {
              print(keyboardRect.height)
            lowerConstraint.constant = keyboardRect.height + 10
        }
    }
    
    @IBAction func PostMessage(_ sender: Any) {
        if let text = textfield.text {
            if !text.isEmpty {
                var ref: DocumentReference?
                ref = db.collection("messages").addDocument(data: [
                    "content": text,
                    "time": Timestamp.init(),
                    "username": username
                ]) { err in
                    if err != nil {
                        print("Error posting message.")
                    } else {
                        print("Message posted with message ID: \(ref!.documentID)")
                        self.textfield.text = ""
                    }
                }
            }
        }
    }
}

extension MessageViewController: UICollectionViewDataSource {
    func numberOfSections(in collectionView: UICollectionView) -> Int {
        return 1
    }
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return messages.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let message = messages[indexPath.item]
        let formatter = DateFormatter()
        formatter.dateStyle = .short
        formatter.timeStyle = .short
        let isOwn: Bool = message.username == username
        
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: isOwn ? "OwnMessageCell" : "MessageCell", for: indexPath) as! MessageCell
        
        cell.header.text = "\(message.username) \(formatter.string(from: message.timestamp))"
        cell.content.text = message.content
        cell.setIsOwnMessage(isOwn: isOwn)
        
        return cell
    }
}

extension MessageViewController: UICollectionViewDelegate {
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        print("Message view was tapped.")
    }
}
