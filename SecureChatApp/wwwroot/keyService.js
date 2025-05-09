class KeyService {
    constructor() {

        if (KeyService.instance) {
            return KeyService.instance;
        }

        this.privateKey = null;
        this.publicKey = null;
        this.sharedKey = null;

        KeyService.instance = this;
    }

    // Generate the ECDH key pair (private and public)
    async generateKeyPair() {
        if (this.publicKey && this.privateKey) {
            return;
        }
        const keyPair = await window.crypto.subtle.generateKey({
            name: "ECDH", namedCurve: "P-256"
        }, true, ["deriveKey", "deriveBits"]);


        this.privateKey = keyPair.privateKey;
        this.publicKey = keyPair.publicKey;
    }

    // Derive the shared key from the client's private key and the other party's public key
    async deriveSharedKey(otherPublicKey) {
        const sharedKey = await window.crypto.subtle.deriveKey({
            name: "ECDH", public: otherPublicKey
        }, this.privateKey, {
            name: "AES-GCM", length: 256,
        }, false, ["encrypt", "decrypt"]);
        this.sharedKey = sharedKey;
    }

    // Import public key from Base64 and convert it to a CryptoKey for use
    async importPublicKeyFromBase64(base64Key) {
        try {
            const keyBuffer = Uint8Array.from(atob(base64Key), c => c.charCodeAt(0));
            const importedKey = await window.crypto.subtle.importKey("spki", keyBuffer.buffer, {
                name: "ECDH", namedCurve: "P-256"
            }, false, []);
            return importedKey;
        } catch (error) {
            console.error("Error importing public key:", error);
            throw error;
        }
    }

    // Export public key as Base64 for transmission over WebSocket
    async getPublicKeyBase64() {
        try {

            if (!(this.publicKey instanceof CryptoKey)) {
                throw new Error("Public key is not a valid CryptoKey object.");
            }
            return this.arrayBufferToBase64(await window.crypto.subtle.exportKey("spki", this.publicKey));
        } catch (error) {
            console.error("Error exporting public key:", error);
            throw error;
        }
    }

    // Convert an ArrayBuffer to a Base64 string
    async arrayBufferToBase64(buffer) {
        return btoa(String.fromCharCode(...new Uint8Array(buffer)));
    }

    // Encrypt the message
    async encryptMessage(message) {
        const encoder = new TextEncoder();
        const data = encoder.encode(message);

        // Generate random IV
        const iv = window.crypto.getRandomValues(new Uint8Array(12));

        const ciphertext = await window.crypto.subtle.encrypt(
            {name: "AES-GCM", iv: iv},
            this.sharedKey,
            data
        );

        return {
            ciphertext: await this.arrayBufferToBase64(ciphertext),
            iv: await this.arrayBufferToBase64(iv)
        };
    }

    // Convert a Base64 string to an ArrayBuffer
    base64ToArrayBuffer(base64) {
        const binaryString = atob(base64); 
        const length = binaryString.length;
        const arrayBuffer = new Uint8Array(length);

        for (let i = 0; i < length; i++) {
            arrayBuffer[i] = binaryString.charCodeAt(i);
        }

        return arrayBuffer;
    }


    // Decrypt a message using the derived shared key
    async decryptMessage(ciphertext, iv) {
        const cipherArrayBuffer = this.base64ToArrayBuffer(ciphertext);
        const ivArray = this.base64ToArrayBuffer(iv);

        const decrypted = await window.crypto.subtle.decrypt(
            { name: "AES-GCM", iv: ivArray },
            this.sharedKey,
            cipherArrayBuffer
        );

        return new TextDecoder().decode(decrypted);
    }
}


const keyService = new KeyService();
export {keyService};