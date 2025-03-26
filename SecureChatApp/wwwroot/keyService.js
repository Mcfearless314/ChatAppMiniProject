class KeyService {
    constructor() {

        if (KeyService.instance) {
            return KeyService.instance;
        }
        
        this.privateKey = null; // Will be stored in memory
        this.publicKey = null; // Will be stored in memory
        this.sharedKey = null; // Derived key, stored in memory

        KeyService.instance = this;
    }

    // Generate the ECDH key pair (private and public)
    async generateKeyPair() {
        if (this.publicKey && this.privateKey) {
            return; // Keys already generated
        }
        const keyPair = await window.crypto.subtle.generateKey(
            {
                name: "ECDH",
                namedCurve: "P-256" // Elliptic curve used for Diffie-Hellman
            },
            true, // Extractable (can export keys)
            ["deriveKey", "deriveBits"] // Usages for the keys
        );

        // Store the private key in memory (DO NOT store it in localStorage!)
        this.privateKey = keyPair.privateKey;
        this.publicKey = keyPair.publicKey;
    }

    // Derive the shared key from the client's private key and the other party's public key
    async deriveSharedKey(otherPublicKey) {
        console.log("otherpublickey", otherPublicKey);
        const sharedKey = await window.crypto.subtle.deriveKey(
            {
                name: "ECDH",
                public: otherPublicKey // The other party's public key
            },
            this.privateKey, // Our private key
            {
                name: "AES-GCM", // The encryption algorithm we will use
                length: 256, // AES 256-bit key
            },
            false, // Extractable (key cannot be extracted)
            ["encrypt", "decrypt"] // Usages for the key
        );

        // Store the derived shared key in memory
        this.sharedKey = sharedKey;
    }

    async importPublicKeyFromBase64(base64Key) {
        try {
            // Convert the Base64 string back to ArrayBuffer
            const keyBuffer = Uint8Array.from(atob(base64Key), c => c.charCodeAt(0));

            // Import the ArrayBuffer back to a CryptoKey
            const importedKey = await window.crypto.subtle.importKey(
                "spki", // Format (SPKI for public key)
                keyBuffer.buffer, // The ArrayBuffer representing the key
                {
                    name: "ECDH", // The algorithm
                    namedCurve: "P-256" // The curve used for ECDH (ensure this matches the key's curve)
                },
                false, // The key is not extractable
                [] // Usages of the key
            );

            console.log("Imported public key:", importedKey);
            return importedKey;
        } catch (error) {
            console.error("Error importing public key:", error);
            throw error;
        }
    }

    // Example method to get the public key in a usable format (e.g., Base64)
    async getPublicKeyBase64() {
        try {
            // Ensure the publicKey is of type CryptoKey
            if (!(this.publicKey instanceof CryptoKey)) {
                throw new Error("Public key is not a valid CryptoKey object.");
            }

            // Export the public key from the CryptoKey object
            const exportedPublicKey = await window.crypto.subtle.exportKey(
                "spki", // Subject Public Key Info format (for public keys)
                this.publicKey
            );

            // Convert the exported key to Base64 format
            return btoa(String.fromCharCode(...new Uint8Array(exportedPublicKey)));
        } catch (error) {
            console.error("Error exporting public key:", error);
            throw error;
        }
    }

    // Encrypt a message using the derived shared key
    async encryptMessage(message) {
        console.log("sharedKey", this.sharedKey);
        const encoder = new TextEncoder();
        const data = encoder.encode(message);

        const iv = window.crypto.getRandomValues(new Uint8Array(12)); // Generate random IV

        const ciphertext = await window.crypto.subtle.encrypt(
            {
                name: "AES-GCM",
                iv: iv, // Initialization vector
            },
            this.sharedKey, // The shared key
            data // The data to encrypt
        );

        return { ciphertext, iv };
    }

    // Decrypt a message using the derived shared key
    async decryptMessage(encryptedData, iv) {
        const decrypted = await window.crypto.subtle.decrypt(
            {
                name: "AES-GCM",
                iv: iv,
            },
            this.sharedKey, // The shared key
            encryptedData
        );

        const decoder = new TextDecoder();
        return decoder.decode(decrypted);
    }
}

// Instantiate the service to manage keys
const keyService = new KeyService();
export { keyService };