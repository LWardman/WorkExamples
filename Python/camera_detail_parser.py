import subprocess


class CameraDetailParser():
    def __init__(self, webcam_dev: str):
        self.webcam_dev: str = str(webcam_dev) # default is /dev/video0
        self.width = None
        self.height = None
        self.fps = None

    def get(self):
        w, h, fps = self.get_camera_formats()
        self.width, self.height, self.fps = w, h, fps
        return w, h, fps
    
    def get_camera_formats(self):
        command = ["v4l2-ctl", "--list-formats-ext", "-d", self.webcam_dev]
        process = subprocess.run(command, capture_output=True, text=True)
        lines = process.stdout.split("\n")
    
        start_idx = 0
        end_idx = 0
        for idx, line in enumerate(lines):
            if "MJPG" in line:
                start_idx = idx            
            if "YUYV" in line:
                end_idx = idx

        lines = lines[start_idx + 1:end_idx]

        candidates = []
        for idx, line in enumerate(lines):
            if "Size" in line:
                w, h = self.get_resolution_from_line(line)
                next_line = lines[idx +1]
                fps = self.get_fps_from_line(next_line)
                candidates.append([w, h, fps])

        best_resolution = self.get_biggest_resolution(candidates)
        return best_resolution

    def get_resolution_from_line(self, line: str):
        words = line.split()
        w, h = words[2].split("x") # idx 2 == resolution
        return w, h

    def get_fps_from_line(self, line: str):
        words = line.split()
        split = words[3].split(".000")
        fps = split[0].split("(")
        return fps[1]
    
    def get_biggest_resolution(self, candidates: list[list[str]]):
        biggest_resolution = max((int(w), int(h), int(fps)) for w, h, fps in candidates)
        w, h, fps = biggest_resolution
        return w, h, fps
