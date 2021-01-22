import cv2
import numpy as np


class MotionDetection:
    """Designated class for motion tracking."""

    def __init__(self):
        self.cap = cv2.VideoCapture(0)
        self.scaling_factor = 0.5

    def frame_diff(self, prev_frame, cur_frame, next_frame):
        diff_frame_1 = cv2.absdiff(next_frame, cur_frame)
        diff_frame_2 = cv2.absdiff(cur_frame, prev_frame)

        return cv2.bitwise_and(diff_frame_1, diff_frame_2)

    def get_frame(self, gray=False):
        _, frame = self.cap.read()
        frame = cv2.resize(frame, None, fx=self.scaling_factor, fy=self.scaling_factor, interpolation=cv2.INTER_AREA)

        if gray:
            frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        return frame

    def put_text(self, frame, text):
        font = cv2.FONT_HERSHEY_SIMPLEX
        font_scale = 1
        color = (0, 255, 255)
        line_type = 2
        padding = 20
        pos = (0 + padding, frame.shape[0] - padding)
        cv2.putText(frame, text, pos, font, font_scale, color, lineType=line_type, thickness=2)

    def start(self):
        # Frame differencing
        prev_frame = self.get_frame(gray=True)
        cur_frame = self.get_frame(gray=True)
        next_frame = self.get_frame(gray=True)
        key_pressed = None

        winname = 'Motion Detection'
        x = 40
        y = 30
        cv2.namedWindow(winname)
        cv2.moveWindow(winname, x, y)

        # Background subtraction
        bg_subtractor = cv2.createBackgroundSubtractorMOG2()
        history = 100
        learning_rate = 1.0 / history

        # Optical Flow
        num_frames_to_track = 5
        num_frames_jump = 2
        tracking_paths = []
        frame_index = 0
        tracking_params = dict(winSize=(11, 11), maxLevel=2, criteria=(cv2.TERM_CRITERIA_EPS | cv2.TERM_CRITERIA_COUNT, 10, 0.03))

        while True:
            frame = self.get_frame()
            frame_gray = self.get_frame(gray=True)

            # Frame differencing
            diff = self.frame_diff(prev_frame, cur_frame, next_frame)
            diff = cv2.cvtColor(diff, cv2.COLOR_GRAY2BGR)
            self.put_text(diff, 'Frame Differencing')

            prev_frame = cur_frame
            cur_frame = next_frame
            next_frame = frame_gray

            # Color spaces
            hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
            lower = np.array([0, 70, 60])
            upper = np.array([50, 150, 255])
            mask = cv2.inRange(hsv, lower, upper)
            img_bitwise_and = cv2.bitwise_and(frame, frame, mask=mask)
            img_median_blurred = cv2.medianBlur(img_bitwise_and, 5)
            self.put_text(img_median_blurred, 'Color Spaces')

            # Background subtraction
            mask = bg_subtractor.apply(frame, learningRate=learning_rate)
            mask = cv2.cvtColor(mask, cv2.COLOR_GRAY2BGR)
            bgs = mask & frame
            self.put_text(bgs, "Background Subtraction")

            # Optical Flow
            of_output = frame.copy()
            if len(tracking_paths) > 0:
                prev_img, current_img = prev_gray, frame_gray
                feature_points_0 = np.float32([tp[-1] for tp in tracking_paths]).reshape(-1, 1, 2)
                feature_points_1, _, _ = cv2.calcOpticalFlowPyrLK(prev_img, current_img, feature_points_0, None, **tracking_params)
                feature_points_0_rev, _, _ = cv2.calcOpticalFlowPyrLK(current_img, prev_img, feature_points_1, None, **tracking_params)
                diff_feature_points = abs(feature_points_0 - feature_points_0_rev).reshape(-1, 2).max(-1)
                good_points = diff_feature_points < 1
                new_tracking_paths = []

                for tp, (x, y), good_points_flag in zip(tracking_paths, feature_points_1.reshape(-1, 2), good_points):
                    if not good_points_flag:
                        continue

                    tp.append((x, y))
                    if len(tp) > num_frames_to_track:
                        del tp[0]
                    new_tracking_paths.append(tp)
                    cv2.circle(of_output, (x, y), 3, (0, 255, 0), -1)

                tracking_paths = new_tracking_paths
                cv2.polylines(of_output, [np.int32(tp) for tp in tracking_paths], False, (0, 150, 0))

            if not frame_index % num_frames_jump:
                of_mask = np.zeros_like(frame_gray)
                of_mask[:] = 255

                for x, y in [np.int32(tp[-1]) for tp in tracking_paths]:
                    cv2.circle(of_mask, (x, y), 6, 0, -1)

                feature_points = cv2.goodFeaturesToTrack(frame_gray, mask=of_mask, maxCorners=500, qualityLevel=0.3, minDistance=7, blockSize=7)

                if feature_points is not None:
                    for x, y in np.float32(feature_points).reshape(-1, 2):
                        tracking_paths.append([(x, y)])

            frame_index += 1
            prev_gray = frame_gray
            self.put_text(of_output, 'Optical Flow')

            # Show
            vis1 = np.concatenate((of_output, diff), axis=1)
            vis2 = np.concatenate((img_median_blurred, bgs), axis=1)
            vis = np.concatenate((vis1, vis2), axis=0)

            cv2.imshow(winname, vis)

            key_pressed = cv2.waitKey(5)

            if key_pressed == 27 or key_pressed == 123 or key_pressed == 124:
                break

        self.cap.release()
        cv2.destroyAllWindows()

        return key_pressed
