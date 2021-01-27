import * as functions from 'firebase-functions'
import * as admin from 'firebase-admin'
try {
    admin.initializeApp()
} catch (error) {
    console.log(error)
}
const db = admin.firestore()

exports.createUser = functions.auth.user().onCreate(async (user) => {
    await db.collection('users').doc(user.uid).set({
        email: user.email,
        createdAt: user.metadata.creationTime
      }).then(result => console.log(result))
})