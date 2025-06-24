async function getKey() {
    
    return await crypto.subtle.generateKey(
        {
            name: "RSASSA-PKCS1-v1_5",
            modulusLength: 2048, 
            publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
            hash: {name: "SHA-256"}, 
        },
        false, 
        ["sign", "verify"] 
    ).then(function(keys){     
        return window.crypto.subtle.exportKey("spki",keys.publicKey);
    }).then (function(keydata){
        return spkiToPEM(keydata);
    });
}
    
function spkiToPEM(keydata){
    var keydataS = arrayBufferToString(keydata);
    var keydataB64 = window.btoa(keydataS);
    var keydataB64Pem = keydataB64;
    return keydataB64Pem;
}

function arrayBufferToString(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode( bytes[ i ] );
    }
    return binary;
}

function exportJsonToFile(content, fileName, contentType) {
    var a = document.createElement("a");
    var file = new Blob([content], {type: contentType});
    a.href = URL.createObjectURL(file);
    a.download = fileName;
    a.click();
}


async function genAES(password, salt) {
    const encoder = new TextEncoder();
    const passwordEncoded = encoder.encode(password);

    const passwordKey = await crypto.subtle.importKey(
        "raw",
        passwordEncoded,
        "PBKDF2",
        false,
        ["deriveKey"]
    );

    return await crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: 1,
            hash: "SHA-256"
        },
        passwordKey,
        {
            name: "AES-GCM",
            length: 256
        },
        true,
        ["encrypt", "decrypt"]
    );
}

async function encryptWithPublicKey(plaintext, password) {
    const encoder = new TextEncoder();
    const data = encoder.encode(plaintext);
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const salt = crypto.getRandomValues(new Uint8Array(16));
    const key = await genAES(password, salt);

    const ciphertext = await crypto.subtle.encrypt(
        {
            name: "AES-GCM",
            iv: iv
        },
        key,
        data
    );

    const combined = new Uint8Array(16 + 12 + ciphertext.byteLength);
    combined.set(salt);
    combined.set(iv, 16);
    combined.set(new Uint8Array(ciphertext), 16 + 12);

    return _arrayBufferToBase64(combined);
}

function _arrayBufferToBase64( bytes ) {
    var binary = '';
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode( bytes[ i ] );
    }
    return window.btoa(binary);
}


async function decryptWithPublicKey(enrcyptdata, password) {
    const combinedArray = new Uint8Array(
        atob(enrcyptdata)
            .split('')
            .map((char) => char.charCodeAt(0))
    );

    const salt = combinedArray.slice(0, 16);
    const iv = combinedArray.slice(16, 28);
    const ciphertextArray = combinedArray.slice(28);
    const key = await genAES(password, salt);

    const decryptedData = await crypto.subtle.decrypt(
        {
            name: 'AES-GCM',
            iv: iv,
        },
        key,
        ciphertextArray
    );

    const decoder = new TextDecoder();
    return decoder.decode(decryptedData);
}