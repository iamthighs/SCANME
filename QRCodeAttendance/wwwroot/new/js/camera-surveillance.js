// Get references to HTML elements
const video1 = document.getElementById("video1");
const video2 = document.getElementById("video2");
const video3 = document.getElementById("video3");
const video4 = document.getElementById("video4");
const video1All = document.getElementById("video1-all");
const video2All = document.getElementById("video2-all");
const video3All = document.getElementById("video3-all");
const video4All = document.getElementById("video4-all");
const startButton1 = document.getElementById("startButton1");
const startButton2 = document.getElementById("startButton2");
const startButton3 = document.getElementById("startButton3");
const startButton4 = document.getElementById("startButton4");
const stopButton1 = document.getElementById("stopButton1");
const stopButton2 = document.getElementById("stopButton2");
const stopButton3 = document.getElementById("stopButton3");
const stopButton4 = document.getElementById("stopButton4");
const captureButton1 = document.getElementById("captureButton1");
const captureButton2 = document.getElementById("captureButton2");
const captureButton3 = document.getElementById("captureButton3");
const captureButton4 = document.getElementById("captureButton4");
const recordButton1 = document.getElementById("recordButton1");
const recordButton2 = document.getElementById("recordButton2");
const recordButton3 = document.getElementById("recordButton3");
const recordButton4 = document.getElementById("recordButton4");
const sourceSelect1 = document.getElementById("sourceSelect1");
const sourceSelect2 = document.getElementById("sourceSelect2");
const sourceSelect3 = document.getElementById("sourceSelect3");
const sourceSelect4 = document.getElementById("sourceSelect4");
const resolutionSelect1 = document.getElementById("resolutionSelect1");
const resolutionSelect2 = document.getElementById("resolutionSelect2");
const resolutionSelect3 = document.getElementById("resolutionSelect3");
const resolutionSelect4 = document.getElementById("resolutionSelect4");

// Function to create a new MediaRecorder instance for each stream
function createMediaRecorder(stream) {
    const mediaRecorder = new MediaRecorder(stream);
    let chunks = [];

    // Handle data available while recording
    mediaRecorder.ondataavailable = function (event) {
        if (event.data.size > 0) {
            chunks.push(event.data);
        }
    };

    // Handle recording stopped
    mediaRecorder.onstop = function () {
        const blob = new Blob(chunks, { type: "video/webm" });
        chunks = [];

        const videoURL = URL.createObjectURL(blob);

        // Create a download link for the recorded video
        const downloadLink = document.createElement("a");
        downloadLink.href = videoURL;
        downloadLink.download = "recorded_video.webm"; // Set the filename here

        // Trigger the download
        downloadLink.click();
    };

    return mediaRecorder;
}

// Function to reset the MediaRecorder instances
function resetMediaRecorders(mediaRecorders) {
    mediaRecorders.forEach((mediaRecorder) => {
        if (mediaRecorder.state === "recording") {
            mediaRecorder.stop();
        }
    });
}

// Check if the browser supports WebRTC and getUserMedia
if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

    // Function to set up video elements and buttons
    function setupVideo(videoElement, startButton, stopButton, sourceSelect, resolutionSelect, captureButton, recordButton) {
        let mediaRecorder;
        let isStarted = false;

        // Function to enable/disable buttons visually
        function toggleButtonsDisabledState() {
            if (isStarted) {
                startButton.classList.add("disabled");
                captureButton.classList.remove("disabled");
                recordButton.classList.remove("disabled");
                stopButton.classList.remove("disabled");
            } else {
                startButton.classList.remove("disabled");
                captureButton.classList.add("disabled");
                recordButton.classList.add("disabled");
                stopButton.classList.add("disabled");
            }
        }

        function resetButtonsToInitialState() {
            isStarted = false;
            videoElement.srcObject = null;
            toggleButtonsDisabledState();
            // ... (other code to reset any other button-specific properties)
        }


        // Record video
        recordButton.addEventListener("click", function () {
            if (isStarted) {
                if (mediaRecorder && mediaRecorder.state === "inactive") {
                    mediaRecorder.start();
                    toastr.success('Recording started...');
                    recordButton.textContent = `Stop Record`;
                } else if (mediaRecorder && mediaRecorder.state === "recording") {
                    mediaRecorder.stop();
                    toastr.error('Recording stopped...');
                    recordButton.textContent = `Record`;
                }
            }
        });

        // Start video streaming
        startButton.addEventListener("click", function () {
            const selectedSource = sourceSelect.value;
            const selectedResolution = resolutionSelect.value;
            const constraints = {
                video: {
                    deviceId: { exact: selectedSource },
                    width: { exact: selectedResolution },
                },
            };

            navigator.mediaDevices
                .getUserMedia(constraints)
                .then(function (stream) {
                    videoElement.srcObject = stream;
                    videoElement.play();
                    toastr.success('Starting...');
                    startButton.disabled = true;
                    stopButton.disabled = false;
                    isStarted = true;
                    toggleButtonsDisabledState();

                    // Create a new MediaRecorder instance for this stream
                    mediaRecorder = createMediaRecorder(stream);

                })
                .catch(function (error) {
                    toastr.error("Error accessing camera: ", error)
                    console.error("Error accessing camera: ", error);
                });
        });

        // Stop video streaming
        stopButton.addEventListener("click", function () {
            if (isStarted) {
                if (videoElement.srcObject) {
                    const tracks = videoElement.srcObject.getTracks();
                    tracks.forEach(function (track) {
                        track.stop();
                        toastr.error('Stop...');
                    });
                    videoElement.srcObject = null;
                    startButton.disabled = false;
                    stopButton.disabled = true;
                    resetButtonsToInitialState();

                    // Reset all MediaRecorder instances
                    resetMediaRecorders([mediaRecorder]);
                }
            }
        });

        captureButton.addEventListener("click", function () {
            if (isStarted) {
                const canvas = document.createElement("canvas");
                canvas.width = videoElement.videoWidth;
                canvas.height = videoElement.videoHeight;
                const context = canvas.getContext("2d");
                context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
                toastr.success('Captured');
                // Create an image element with the captured frame
                const capturedImage = document.createElement("img");
                capturedImage.src = canvas.toDataURL("image/png");

                // Create a download link for the captured image
                const downloadLink = document.createElement("a");
                downloadLink.href = capturedImage.src;
                downloadLink.download = "captured_image.png";

                // Trigger the download
                downloadLink.click();
            }
        });

        // Populate video source options
        function populateSourceOptions() {
            populateSourceOptionsHelper(sourceSelect);
        }

        toggleButtonsDisabledState();
        populateSourceOptions();
    }

    // Function to set up video elements and buttons for single video without recording
    function setupVideo1(videoElement, startButton, stopButton, sourceSelect, resolutionSelect) {
        let isStarted = false;
        let mediaRecorder; // Added this line

        // Function to enable/disable buttons visually
        function toggleButtonsDisabledState() {
            if (isStarted) {
                startButton.classList.add("disabled");
                stopButton.classList.remove("disabled");
            } else {
                startButton.classList.remove("disabled");
                stopButton.classList.add("disabled");
            }
        }

        function resetButtonsToInitialState() {
            isStarted = false;
            videoElement.srcObject = null;
            toggleButtonsDisabledState();
            // ... (other code to reset any other button-specific properties)
        }


        // Start video streaming
        startButton.addEventListener("click", function () {
            const selectedSource = sourceSelect.value;
            const selectedResolution = resolutionSelect.value;
            const constraints = {
                video: {
                    deviceId: { exact: selectedSource },
                    width: { exact: selectedResolution },
                },
            };

            navigator.mediaDevices
                .getUserMedia(constraints)
                .then(function (stream) {
                    videoElement.srcObject = stream;
                    videoElement.play();
                    startButton.disabled = true;
                    stopButton.disabled = false;
                    isStarted = true;
                    toggleButtonsDisabledState();

                    // Create a new MediaRecorder instance for this stream
                    mediaRecorder = createMediaRecorder(stream);

                })
                .catch(function (error) {
                    console.error("Error accessing camera: ", error);
                });
        });

        // Stop video streaming
        stopButton.addEventListener("click", function () {
            if (isStarted) {
                if (videoElement.srcObject) {
                    const tracks = videoElement.srcObject.getTracks();
                    tracks.forEach(function (track) {
                        track.stop();
                    });
                    videoElement.srcObject = null;
                    startButton.disabled = false;
                    stopButton.disabled = true;
                    resetButtonsToInitialState();

                    // Reset all MediaRecorder instances
                    resetMediaRecorders([mediaRecorder]);
                }
            }
        });

        toggleButtonsDisabledState();
    }

    // Helper function to populate video source options
    function populateSourceOptionsHelper(selectElement) {
        // Clear existing options
        selectElement.innerHTML = "";

        navigator.mediaDevices
            .enumerateDevices()
            .then(function (devices) {
                devices.forEach(function (device) {
                    if (device.kind === "videoinput") {
                        const option = document.createElement("option");
                        option.text = device.label || "Camera " + (selectElement.length + 1);
                        option.value = device.deviceId;
                        selectElement.appendChild(option);
                    }
                });
            })
            .catch(function (error) {
                console.error("Error listing devices: ", error);
            });
    }

    // Set up each video element and buttons
    setupVideo(video1, startButton1, stopButton1, sourceSelect1, resolutionSelect1, captureButton1, recordButton1);
    setupVideo(video2, startButton2, stopButton2, sourceSelect2, resolutionSelect2, captureButton2, recordButton2);
    setupVideo(video3, startButton3, stopButton3, sourceSelect3, resolutionSelect3, captureButton3, recordButton3);
    setupVideo(video4, startButton4, stopButton4, sourceSelect4, resolutionSelect4, captureButton4, recordButton4);

    setupVideo1(video1All, startButton1, stopButton1, sourceSelect1, resolutionSelect1);
    setupVideo1(video2All, startButton2, stopButton2, sourceSelect2, resolutionSelect2);
    setupVideo1(video3All, startButton3, stopButton3, sourceSelect3, resolutionSelect3);
    setupVideo1(video4All, startButton4, stopButton4, sourceSelect4, resolutionSelect4);
} else {
    console.error("getUserMedia is not supported in this browser.");
}