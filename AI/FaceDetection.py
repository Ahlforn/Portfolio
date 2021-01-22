import cv2
import numpy as np


class FaceDetection:

    def __init__(self):
        self.cap = cv2.VideoCapture(0)
        self.scaling_factor = 0.5

    def start(self):
        winname = 'Face Detection'
        x = 40
        y = 30
        cv2.namedWindow(winname)
        cv2.moveWindow(winname, x, y)
        face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

        key_pressed = None

        if face_cascade.empty():
            raise IOError('Unable to load the face cascade classifier xml file')

        while True:
            _, frame = self.cap.read()
            frame = cv2.resize(frame, None, fx=self.scaling_factor, fy=self.scaling_factor, interpolation=cv2.INTER_AREA)
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            face_rects = face_cascade.detectMultiScale(gray, 1.3, 5)

            for (x, y, w, h) in face_rects:
                cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

            cv2.imshow(winname, frame)

            key_pressed = cv2.waitKey(5)

            if key_pressed == 27 or key_pressed == 123 or key_pressed == 124:
                break

        self.cap.release()
        cv2.destroyAllWindows()

        return key_pressed
