using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expand.core.space
{
    /// <summary>
    /// Any GameObject that is saved in a sector should be a SpaceObject.
    /// </summary>
    public abstract class SpaceObject : GameObject
    {
        // Anything that should be saved in a sector file is a space object
        protected bool solid;
        protected float rotation;
        protected float scale;
        protected Color color;
        protected Object collide_object;
        protected Sprite sprite;

        public SpaceObject()
        {
            scale = 1f;
            this.setSprite();
        }

        /// <summary>
        /// Adds self to the sector that created it upon initialization.
        /// </summary>
        /// <param name="sector_inside">The sector that this object is contained in.</param>
        public void addToSector(Sector sector_inside)
        {
            if (sector_inside != null)
            {
                sector_inside.space_objects.Add(this);
            }
        }

        public override void setDead()
        {
            this.alive = false;
        }

        /// <summary>
        /// Deletes SpaceObject from sector.
        /// </summary>
        public void removeFromSector()
        {
            getContainingSector().space_objects.Remove(this);
        }

        /// <summary>
        /// Returns the sector that the SpaceObject is contained in.
        /// </summary>
        /// <returns></returns>
        public Sector getContainingSector()
        {
            int[] sector_coords = Space.getSector(pos[0], pos[1]);
            return Program.game.space.findLoadedSector(sector_coords[0], sector_coords[1]);
        }

        /// <summary>
        /// Overridable method that is dispatched on collision. The colliding SpaceObject is passed as an argument.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void onCollide(SpaceObject obj)
        {

        }

        /// <summary>
        /// Move should be called whenever an inheriting SpaceObject wants to move. Collision detection occurs and onCollide methods are dispatched.
        /// </summary>
        /// <param name="delta_x">Change in X</param>
        /// <param name="delta_y">Change in Y</param>
        /// <returns></returns>
        public bool move(int delta_x, int delta_y)
        {
            this.pos[0] += delta_x;
            this.pos[1] += delta_y;
            bool success = true;
            foreach (SpaceObject obj in getContainingSector().space_objects)
            {
                if (obj.collidesWith(this)) // Use their collision method, not ours
                {
                    success = false;
                    this.onCollide(obj);
                    obj.onCollide(this);
                    break;
                }
            }
            if (!success)
            {
                this.pos[0] -= delta_x;
                this.pos[1] -= delta_y;
            }
            return success;
        }

        /// <summary>
        /// Get a transformation vector for use in collision detection.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private Matrix getTransform(int[] pos, Vector2 origin, float scale, float rotation)
        {
            Matrix transform =
                    Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateScale(scale) *
                    Matrix.CreateRotationZ(rotation) *
                    Matrix.CreateTranslation(new Vector3(pos[0], pos[1], 0.0f));
            return transform;
        }

        /// <summary>
        /// Must be implemented to be able to collide with other SpaceObjects.
        /// </summary>
        /// <returns>Any collision object that has fully implemented collision methods.</returns>
        public virtual bool collidesWith(SpaceObject obj)
        {
            // Check if thing is solid
            if (!obj.solid || !this.solid)
            {
                return false;
            }

            int dist = (int)Util.distance(this.pos[0], this.pos[1], obj.pos[0], obj.pos[1]);
            if(dist > (obj.sprite.frame.Height + obj.sprite.frame.Width) / 2 + (this.sprite.frame.Height + this.sprite.frame.Width) / 2)
            {
                return false;
            }

            Matrix tranformA = getTransform(this.pos, new Vector2(this.sprite.frame.Height / 2, this.sprite.frame.Width / 2), this.scale, this.rotation);
            Matrix tranformB = getTransform(obj.pos, new Vector2(obj.sprite.frame.Height / 2, obj.sprite.frame.Width / 2), obj.scale, obj.rotation);

            bool intersects = IntersectPixels(tranformA, this.sprite.frame.Width, this.sprite.frame.Height, this.sprite.mesh, tranformB, obj.sprite.frame.Width, obj.sprite.frame.Height, obj.sprite.mesh);

            return intersects;
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                        Matrix transformA, int widthA, int heightA, Color[] dataA,
                        Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        public abstract void setSprite();
    }
}
