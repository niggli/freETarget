﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freETarget.targets {
    class Pistol25mRF : aTarget {

        private decimal pelletCaliber;
        private const decimal targetSize = 550; //mm
        private const int pistolBlackRings = 5;
        private const bool solidInnerTenRing = false;

        private const int trkZoomMin = 1;
        private const int trkZoomMax = 5;
        private const int trkZoomVal = 1;
        private const decimal pdfZoomFactor = 1;

        private const decimal outterRing = 500m; //mm
        private const decimal ring6 = 420m; //mm
        private const decimal ring7 = 340m; //mm
        private const decimal ring8 = 260m; //mm
        private const decimal ring9 = 180m; //mm
        private const decimal ring10 = 100m; //mm
        private const decimal innerRing = 50m; //mm

        private decimal innerTenRadiusPistol;

        private static readonly decimal[] ringsPistol = new decimal[] { outterRing, ring6, ring7, ring8, ring9, ring10, innerRing };


        public Pistol25mRF(decimal caliber) : base(caliber) {
            this.pelletCaliber = caliber;
            innerTenRadiusPistol = innerRing / 2m + pelletCaliber / 2m;
        }
        public override int getBlackRings() {
            return pistolBlackRings;
        }

        public override decimal getInnerTenRadius() {
            return innerTenRadiusPistol;
        }

        public override decimal getOutterRadius() {
            return getOutterRing() / 2m + pelletCaliber / 2m;
        }

        public override decimal get9Radius() {
            return ring9 / 2m + pelletCaliber / 2m;
        }
        public override string getName() {
            return typeof(Pistol25mRF).FullName;
        }

        public override decimal getOutterRing() {
            return outterRing;
        }

        public override float getFontSize(float diff) {
            return diff / 18f; 
        }

        public override decimal getPDFZoomFactor(List<Shot> shotList) {
            if (shotList == null) {
                return pdfZoomFactor;
            } else {
                bool zoomed = true;
                foreach (Shot s in shotList) {
                    if (s.score < 6) {
                        zoomed = false;
                    }
                }

                if (zoomed) {
                    return 0.5m;
                } else {
                    return 1;
                }
            }
        }

        public override decimal getProjectileCaliber() {
            return pelletCaliber;
        }

        public override decimal[] getRings() {
            return ringsPistol;
        }

        public override decimal getSize() {
            return targetSize;
        }

        public override int getTrkZoomMaximum() {
            return trkZoomMax;
        }

        public override int getTrkZoomMinimum() {
            return trkZoomMin;
        }

        public override int getTrkZoomValue() {
            return trkZoomVal;
        }

        public override decimal getZoomFactor(int value) {
            return (decimal)(1 / (decimal)value);
        }

        public override bool isSolidInner() {
            return solidInnerTenRing;
        }

        public override decimal getBlackDiameter() {
            return outterRing;
        }

        public override int getRingTextCutoff() {
            return 9;
        }
        public override float getTextOffset(float diff, int ring) {
            //return diff / 4;
            return 0;
        }


        public override int getTextRotation() {
            return 0;
        }

        public override int getFirstRing() {
            return 5;
        }

        public override bool isRapidFire() {
            return true;
        }
    }
}
