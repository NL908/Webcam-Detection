import React, { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [imageSrc, setImageSrc] = useState('');
    const [imageRes, setImageRes] = useState('');

    useEffect(() => {
        const ws = new WebSocket('ws://localhost:8080/ws');

        ws.onmessage = (event) => {
            const json = JSON.parse(event.data);
            setImageSrc(`data:image/png;base64,${json.srcImageBase64Data}`);
            setImageRes(`data:image/png;base64,${json.resImageBase64Data}`)
        };

        return () => {
            ws.close();
        };
    }, []);

    return (
        imageSrc ? 
            <div className="row">
                {imageSrc &&
                    <img src={imageSrc} alt="Source Image from WebSocket"/>
                }
                {imageRes && 
                    <img src={imageRes} alt="Result Image from WebSocket"/>
                }
            </div>
        :
            <div>
                Loading backend
            </div>
    );
}

export default App
