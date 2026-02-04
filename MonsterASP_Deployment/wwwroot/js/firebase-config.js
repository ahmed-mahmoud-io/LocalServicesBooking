// Import the functions you need from the SDKs you need
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-app.js";
import { getAnalytics } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-analytics.js";
import { getAuth, GoogleAuthProvider, FacebookAuthProvider, signInWithPopup } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-auth.js";
import { getFirestore } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-firestore.js";
import { getStorage } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-storage.js";
import { getMessaging } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js";

// Your web app's Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyC94TdPzeQEsJikw2e5a21TsYhhKD-EA60",
    authDomain: "booking-services-97a38.firebaseapp.com",
    projectId: "booking-services-97a38",
    storageBucket: "booking-services-97a38.firebasestorage.app",
    messagingSenderId: "640126311891",
    appId: "1:640126311891:web:201dfea2edf6fec0b5e326",
    measurementId: "G-6BD6T5WQWV"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);
const auth = getAuth(app);
const db = getFirestore(app);
const storage = getStorage(app);
// Messaging might require service worker registration, so we export it conditionally or wait for it
// const messaging = getMessaging(app); 

export { app, analytics, auth, db, storage, GoogleAuthProvider, FacebookAuthProvider, signInWithPopup };
