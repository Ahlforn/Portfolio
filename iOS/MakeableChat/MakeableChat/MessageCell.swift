//
//  MessageCell.swift
//  MakeableChat
//
//  Created by Anders Hofmeister on 28/01/2021.
//

import UIKit

class MessageCell: UICollectionViewCell {
    let header = UILabel()
    let content = PaddedLabel()
    var ref: UILayoutGuide?
    
    override init(frame: CGRect) {
        super.init(frame: frame)
        
        ref = contentView.readableContentGuide
        
        header.font = header.font.withSize(12)
        header.translatesAutoresizingMaskIntoConstraints = false
        
        contentView.addSubview(header)
        contentView.addSubview(content)
        content.lineBreakMode = .byWordWrapping
        content.numberOfLines = 0
        content.layer.cornerRadius = 10
        content.layer.masksToBounds = true
        content.translatesAutoresizingMaskIntoConstraints = false
        
        if let ref = ref {
            header.addConstraint(NSLayoutConstraint(item: header, attribute: .height, relatedBy: .equal, toItem: nil, attribute: .notAnAttribute, multiplier: 1, constant: 21))
            header.topAnchor.constraint(equalTo: ref.topAnchor).isActive = true
            content.topAnchor.constraint(equalTo: header.bottomAnchor).isActive = true
            ref.bottomAnchor.constraint(equalTo: content.bottomAnchor).isActive = true
        }
    }
    
    func setIsOwnMessage(isOwn: Bool) {
        if let ref = ref {
            if isOwn{
                NSLayoutConstraint.activate([
                    ref.rightAnchor.constraint(equalTo: header.rightAnchor),
                    content.leftAnchor.constraint(equalToSystemSpacingAfter: ref.leftAnchor, multiplier: 10),
                    ref.rightAnchor.constraint(equalTo: content.rightAnchor)
                ])
                content.backgroundColor = .systemBlue
            } else {
                NSLayoutConstraint.activate([
                    header.leftAnchor.constraint(equalTo: ref.leftAnchor),
                    content.leftAnchor.constraint(equalTo: ref.leftAnchor),
                    ref.rightAnchor.constraint(equalToSystemSpacingAfter: content.rightAnchor, multiplier: 10)
                ])
                content.backgroundColor = .systemGray
            }
        }
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented.")
    }
}

@IBDesignable class PaddedLabel: UILabel {
    
    @IBInspectable var topInset: CGFloat = 5.0
    @IBInspectable var bottomInset: CGFloat = 5.0
    @IBInspectable var leftInset: CGFloat = 12.0
    @IBInspectable var rightInset: CGFloat = 12.0
    
    override func drawText(in rect: CGRect) {
        let insets = UIEdgeInsets.init(top: topInset, left: leftInset, bottom: bottomInset, right: rightInset)
        super.drawText(in: rect.inset(by: insets))
    }
    
    override var intrinsicContentSize: CGSize {
        let size = super.intrinsicContentSize
        return CGSize(width: size.width + leftInset + rightInset,
                      height: size.height + topInset + bottomInset)
    }
}
