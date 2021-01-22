import pafy
import youtube_dl
import cv2
import numpy as np

class PedestrianDetection:
    """Designated class for detection of pedestrians"""
    """
    From https://stackoverflow.com/questions/60973425/youtube-dl-is-updated-to-latest-version-when-i-am-running-this-code-this-long-we

    do a global search for a file named Install Certificates.command. It is located inside python folder.
    Double click on it and the error should disappear
    """

    def __init__(self, url=None, images=None):
        self.url = url
        self.pafy = pafy.new(url)
        self.video = self.pafy.getbest(preftype='mp4')
        self.cap = cv2.VideoCapture(self.video.url)
        self.scaling_factor = 0.75

        self.colorRanges = [
            ([17, 15, 100], [50, 56, 200]),
            ([86, 31, 4], [220, 88, 50]),
            ([25, 146, 190], [62, 174, 250]),
            ([103, 86, 65], [145, 133, 128])
        ]

    def start(self):
        hog = cv2.HOGDescriptor()
        hog.setSVMDetector(cv2.HOGDescriptor_getDefaultPeopleDetector())

        winname = 'Pedestrian Detection'
        x = 40
        y = 30
        cv2.namedWindow(winname)
        cv2.moveWindow(winname, x, y)

        key_pressed = None

        while True:
            r, frame = self.cap.read()
            if r:
                frame = cv2.resize(frame, None, fx=self.scaling_factor, fy=self.scaling_factor, interpolation=cv2.INTER_AREA)
                gray_frame = cv2.cvtColor(frame, cv2.COLOR_RGB2GRAY)
                rects, weights = hog.detectMultiScale(gray_frame)

                for i, (x, y, w, h) in enumerate(rects):
                    if weights[i] < 0.7:
                        continue
                    cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

                    detected_colors = []
                    img = frame[y:y + h, x:x + w]
                    for (lower, upper) in self.colorRanges:
                        color = (upper[0], upper[1], upper[2])
                        lower = np.array(lower, dtype=np.uint8)
                        upper = np.array(upper, dtype=np.uint8)
                        hsv = cv2.cvtColor(img, cv2.COLOR_RGB2HSV)
                        mask = cv2.inRange(hsv, lower, upper)
                        mask = cv2.erode(mask, None, iterations=2)
                        mask = cv2.dilate(mask, None, iterations=2)
                        contours = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2]

                        if len(contours) != 0:
                            # color = (
                            #     (lower[0] + upper[0]) / 2,
                            #     (lower[1] + upper[1]) / 2,
                            #     (lower[2] + upper[2]) / 2
                            # )
                            detected_colors.append(color)

                    if len(detected_colors) > 0:
                        cw = w / len(detected_colors)

                        for i, color in enumerate(detected_colors):
                            cv2.rectangle(frame, (int(x + i * cw), y + h - 15), (int(x + i * cw + cw), y + h), color, thickness=-2)


                cv2.imshow(winname, frame)

            key_pressed = cv2.waitKey(1)

            if key_pressed == 27 or key_pressed == 123 or key_pressed == 124:
                break

        cv2.destroyAllWindows()

        return key_pressed
