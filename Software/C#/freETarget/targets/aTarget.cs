﻿using freETarget.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freETarget.targets {
    public abstract class aTarget {


        // -----------  abstract functions to be implemented in child classes ----------

        public abstract string getName();

        public abstract decimal getProjectileCaliber();

        public abstract decimal getSize();

        public abstract decimal[] getRings();

        public abstract decimal getOutterRing();

        public abstract decimal getOutterRadius();

        public abstract decimal get9Radius();

        public abstract decimal getInnerTenRadius();

        public abstract int getBlackRings();

        public abstract decimal getBlackDiameter();

        public abstract decimal getZoomFactor(int zoomValue);

        public abstract bool isSolidInner();

        public abstract int getTrkZoomMinimum();

        public abstract int getTrkZoomMaximum();

        public abstract int getTrkZoomValue();

        public abstract float getFontSize(float diff);

        public abstract int getRingTextCutoff();

        public abstract float getTextOffset(float diff, int ring);

        public abstract decimal getPDFZoomFactor(List<Shot> shotList);

        public abstract int getTextRotation();

        public abstract int getFirstRing();

        public abstract bool isRapidFire();


        //------------ common implementations ----------------------------

        protected aTarget(decimal caliber) {

        }

        public Bitmap paintTarget(int dimension, int zoomValue, bool notConnected, Session currentSession, List<Shot> shotList) {

            bool solidInner = isSolidInner();
            decimal zoomFactor = getZoomFactor(zoomValue);
            int blackRingCutoff = getBlackRings();
            decimal[] rings = getRings();

            float center = (float)(dimension / 2);

            if (dimension == 0) { //window is minimized. nothing to paint
                return null;
            }
            Pen penBlack = new Pen(Color.Black);
            Pen penWhite = new Pen(Settings.Default.targetColor);
            Brush brushBlack = new SolidBrush(Color.Black);
            Brush brushWhite = new SolidBrush(Settings.Default.targetColor);

            Bitmap bmpTarget = new Bitmap(dimension, dimension);
            Graphics it = Graphics.FromImage(bmpTarget);
            it.SmoothingMode = SmoothingMode.AntiAlias;
            it.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            it.FillRectangle(brushWhite, 0, 0, dimension - 1, dimension - 1);

            float circle = getDimension(dimension, getBlackDiameter(), zoomFactor);

            float x = center - (circle / 2);
            it.FillEllipse(brushBlack, x, x, circle, circle); //draw the black circle

            int r = getFirstRing();
            for (int i = 0; i < rings.Length; i++) {

                Pen p;
                Brush bText;
                if (r < blackRingCutoff) {
                    p = penBlack;
                    bText = brushBlack;
                } else {
                    p = penWhite;
                    bText = brushWhite;
                }

                circle = getDimension(dimension, rings[i], zoomFactor);
                
                x = center - (circle / 2);
                float y = center + (circle / 2);

                if (solidInner && i == rings.Length - 1) //rifle target - last ring (10) is a solid dot
                {
                    it.FillEllipse(brushWhite, x, x, circle, circle);
                } else {
                    it.DrawEllipse(p, x, x, circle, circle);
                }

                if (r <= getRingTextCutoff()) 
                {
                    float nextCircle = getDimension(dimension, rings[i + 1], zoomFactor);
                    float diff = circle - nextCircle;
                    float fontSize = getFontSize(diff); 
                    Font f = new Font("Arial", fontSize);

                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;
                   
                    it.TranslateTransform(dimension / 2, dimension / 2); //set coordinates in the middle of the target
                    it.RotateTransform(getTextRotation());
                    it.TranslateTransform(0, -(dimension / 2) + x + (diff / 4) - getTextOffset(diff, r)); //move coordinates at the exact center point of the text
                    it.DrawString(r.ToString(), f, bText, 0, 0, format);
                    it.ResetTransform();

                    it.TranslateTransform(dimension / 2, dimension / 2); //set coordinates in the middle of the target
                    it.RotateTransform(getTextRotation());
                    it.TranslateTransform(0, (dimension / 2) - x - (diff / 4) + getTextOffset(diff, r));
                    if (getTextRotation() > 0) {
                        it.RotateTransform(180);
                    }
                    it.DrawString(r.ToString(), f, bText, 0, 0, format);
                    it.ResetTransform();

                    if (!isRapidFire()) {
                        it.TranslateTransform(dimension / 2, dimension / 2); //set coordinates in the middle of the target
                        it.RotateTransform(getTextRotation());
                        it.TranslateTransform(-(dimension / 2) + y - (diff / 4) + getTextOffset(diff, r), 0);
                        if (getTextRotation() > 0) {
                            it.RotateTransform(90);
                        }
                        it.DrawString(r.ToString(), f, bText, 0, 0, format);
                        it.ResetTransform();

                        it.TranslateTransform(dimension / 2, dimension / 2); //set coordinates in the middle of the target
                        it.RotateTransform(getTextRotation());
                        it.TranslateTransform((dimension / 2) - y + (diff / 4) - getTextOffset(diff, r), 0);
                        if (getTextRotation() > 0) {
                            it.RotateTransform(270);
                        }
                        it.DrawString(r.ToString(), f, bText, 0, 0, format);
                        it.ResetTransform();
                    }
                }
                r++;
            }

            if (currentSession.sessionType == Event.EventType.Practice) {
                //draw triangle in corner
                float sixth = dimension / 6f;
                PointF[] points = new PointF[3];
                points[0].X = 5 * sixth;
                points[0].Y = 0;

                points[1].X = dimension;
                points[1].Y = sixth;

                points[2].X = dimension;
                points[2].Y = 0;
                Brush brushBlue = new SolidBrush(Color.DarkBlue);
                it.FillPolygon(brushBlue, points);
            }

            if (isRapidFire()) {
                //for the RF target, draw 2 lines on left and right 125mm x 5mm

                float bar_height = getDimension(dimension, 5, zoomFactor);
                float bar_width = getDimension(dimension, 125, zoomFactor);

                float bar_y = dimension / 2 - bar_height / 2;

                float leftBar_x = center - getDimension(dimension, getOutterRing() / 2, zoomFactor);
                float rightBar_x = center + getDimension(dimension, getOutterRing() / 2, zoomFactor) - bar_width;

                it.FillRectangle(brushWhite, leftBar_x, bar_y, bar_width, bar_height);
                it.FillRectangle(brushWhite, rightBar_x, bar_y, bar_width, bar_height);
            }

            it.DrawRectangle(penBlack, 0, 0, dimension - 1, dimension - 1);

            int index = 0;
            foreach (Shot shot in shotList) {
                drawShot(shot, it, dimension, zoomFactor, index++, shotList);
            }

            if (Settings.Default.drawMeanGroup) {
                drawMeanGroup(it, dimension, zoomFactor, currentSession, shotList);
            }



            if (notConnected) {
                bmpTarget = toGrayScale(bmpTarget);
            }
            return bmpTarget;
        }

        protected float getDimension(decimal currentTargetSize, decimal milimiters, decimal zoomFactor) {
            return (float)((currentTargetSize * milimiters) / (getSize() * zoomFactor));
        }

        protected Bitmap toGrayScale(Bitmap original) {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap)) {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes()) {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }


        protected void drawShot(Shot shot, Graphics it, int targetSize, decimal zoomFactor, int l, List<Shot> shotList) {

            if(shot.miss == true) {
                return;
            }

            //transform shot coordinates to imagebox coordinates

            PointF x = transform((float)shot.getX(), (float)shot.getY(), targetSize, zoomFactor);

            //draw shot on target
            int count = shotList.Count;

            Color c = Color.FromArgb(200, Settings.Default.scoreOldBackgroundColor); //semitransparent old shots
            Pen p = new Pen(Settings.Default.scoreOldPenColor);
            Brush bText = new SolidBrush(Settings.Default.scoreOldPenColor);


            if (l == count - 1) { //last (current) shot
                if (shot.decimalScore > 9.9m) {
                    c = Settings.Default.score10BackgroundColor;
                    p = new Pen(Settings.Default.score10PenColor);
                    bText = new SolidBrush(Settings.Default.score10PenColor);
                } else if (shot.decimalScore > 8.9m && shot.decimalScore <= 9.9m) {
                    c = Settings.Default.score9BackgroundColor;
                    p = new Pen(Settings.Default.score9PenColor);
                    bText = new SolidBrush(Settings.Default.score9PenColor);
                } else {
                    c = Settings.Default.scoreDefaultBackgroundColor;
                    p = new Pen(Settings.Default.scoreDefaultPenColor);
                    bText = new SolidBrush(Settings.Default.scoreDefaultPenColor);
                }
            }


            Brush b = new SolidBrush(c);


            it.SmoothingMode = SmoothingMode.AntiAlias;
            it.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float peletSize = getDimension(targetSize, getProjectileCaliber(), zoomFactor);

            x.X -= peletSize / 2;
            x.Y -= peletSize / 2;

            it.FillEllipse(b, new RectangleF(x, new SizeF(peletSize, peletSize)));
            it.DrawEllipse(p, new RectangleF(x, new SizeF(peletSize, peletSize)));

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Font f = new Font("Arial", peletSize / 3);

            x.X += 0.2f; //small adjustment for the number to be centered
            x.Y += 1f;
            it.DrawString((shot.index + 1).ToString(), f, bText, new RectangleF(x, new SizeF(peletSize, peletSize)), format);
        }


        private PointF transform(float xp, float yp, float size, decimal zoomFactor) {
            //matrix magic from: https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/samples/jj635757(v=vs.85)

            System.Numerics.Matrix4x4 M = new System.Numerics.Matrix4x4(0, 0, 1, 0,
                                                                        0, 0, 0, 1,
                                                                        size, size, 1, 0,
                                                                       -size, size, 0, 1);

            System.Numerics.Matrix4x4 Minverted;
            System.Numerics.Matrix4x4.Invert(M, out Minverted);

            float shotRange = (float)(getSize() * zoomFactor) / 2f;
            System.Numerics.Matrix4x4 xyPrime = new System.Numerics.Matrix4x4(-shotRange, 0, 0, 0,
                                                                                shotRange, 0, 0, 0,
                                                                                shotRange, 0, 0, 0,
                                                                               -shotRange, 0, 0, 0);

            System.Numerics.Matrix4x4 abcd = System.Numerics.Matrix4x4.Multiply(Minverted, xyPrime);

            float a = abcd.M11;
            float b = abcd.M21;
            float c = abcd.M31;
            float d = abcd.M41;

            float x = (a * xp + b * yp - b * d - a * c) / (a * a + b * b);
            float y = (b * xp - a * yp - b * c + a * d) / (a * a + b * b);

            PointF ret = new PointF(x, y);
            return ret;
        }


        protected void drawMeanGroup(Graphics it, decimal currentTargetSize, decimal zoomFactor, Session currentSession, List<Shot> shotList) {
            if (shotList.Count >= 2) {
                float circle = getDimension(currentTargetSize, currentSession.rbar * 2, zoomFactor);

                PointF x = transform((float)currentSession.xbar, (float)currentSession.ybar, (float)currentTargetSize, zoomFactor);
                Pen p = new Pen(Color.Red, 2);

                it.DrawEllipse(p, x.X - (circle / 2), x.Y - (circle / 2), circle, circle);

                float cross = 5; // center of group cross is always the same size - 5 pixels

                it.DrawLine(p, x.X - cross, x.Y, x.X + cross, x.Y);
                it.DrawLine(p, x.X, x.Y - cross, x.X, x.Y + cross);
            }
        }
    }
}
