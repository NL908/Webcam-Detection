let peerConnection = new RTCPeerConnection();
let sourceStream;
let offer
let answer;

let init = async () => {
    sourceStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });

    document.getElementById('sourceVideo').srcObject = sourceStream;

    sourceStream.getTracks().forEach((track) => {
        peerConnection.addTrack(track, sourceStream);
    })

    peerConnection.onconnectionstatechange = () => {
        if (peerConnection.connectionState === 'connected') {
            alert("WebRTC connected.");
        }
    };
};

let createOffer = async () => {
    peerConnection.onicecandidate = async (event) => {
        //Event that fires off when a new offer ICE candidate is created
        if (event.candidate) {
            document.getElementById('offer-text').value = JSON.stringify(peerConnection.localDescription)
            navigator.clipboard.writeText(JSON.stringify(peerConnection.localDescription));
            alert("Offer copied to clipboard.");
        }
    };
    offer = await peerConnection.createOffer();
    await peerConnection.setLocalDescription(offer);
}

let addAnswer = async () => {
    await navigator.clipboard.readText().then((clipText) => (answer = JSON.parse(clipText)));
    peerConnection.setRemoteDescription(answer);
}

init()
document.getElementById('create-Offer').addEventListener('click', createOffer)
document.getElementById('add-answer').addEventListener('click', addAnswer)